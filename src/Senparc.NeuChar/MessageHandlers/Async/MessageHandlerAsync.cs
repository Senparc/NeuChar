#region Apache License Version 2.0
/*----------------------------------------------------------------

Copyright 2021 Suzhou Senparc Network Technology Co.,Ltd.

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.

Detail: https://github.com/JeffreySu/WeiXinMPSDK/blob/master/license.md

----------------------------------------------------------------*/
#endregion Apache License Version 2.0

/*----------------------------------------------------------------
    Copyright (C) 2021 Senparc
    
    文件名：MessageHandler.cs
    文件功能描述：微信请求【异步方法】的集中处理方法
    
    
    创建标识：Senparc - 20160122

    
    修改标识：Senparc - 20191203
    修改描述：v1.0.104 修改备注。优化 MessageHandler 同步方法兼容策略。

----------------------------------------------------------------*/

/*
 * V4.0 添加异步方法
 * V6.0 转为以异步方法为主
 */

using System;
using System.IO;
using System.Xml.Linq;
using Senparc.NeuChar.Context;
using Senparc.NeuChar.Entities;
using System.Threading.Tasks;
using Senparc.NeuChar.Exceptions;
using Senparc.CO2NET.APM;
using System.Threading;
using Senparc.CO2NET.Trace;
using Senparc.CO2NET.Cache;
using Senparc.CO2NET.MessageQueue;

namespace Senparc.NeuChar.MessageHandlers
{
    /// <summary>
    /// 微信请求的集中处理方法
    /// 此方法中所有过程，都基于Senparc.NeuChar.基础功能，只为简化代码而设。
    /// </summary>
    public abstract partial class MessageHandler<TMC, TRequest, TResponse> : IMessageHandler<TRequest, TResponse>
        where TMC : class, IMessageContext<TRequest, TResponse>, new()
        where TRequest : class, IRequestMessageBase
        where TResponse : class, IResponseMessageBase
    {
        #region 异步方法

        /// <summary>
        /// 默认参数设置为 DefaultResponseMessageAsync
        /// </summary>
        private DefaultMessageHandlerAsyncEvent _defaultMessageHandlerAsyncEvent = DefaultMessageHandlerAsyncEvent.DefaultResponseMessageAsync;

        /// <summary>
        /// <para>注意：当调用同步方法 Execute() 时，此参数会被强制设置为：SelfSynicMethod！</para>
        /// <para>MessageHandler 事件异步方法的默认调用方法（在没有override的情况下）。默认值：DefaultDefaultResponseMessageAsync。</para>
        /// <para>默认参数设置为 DefaultResponseMessageAsync，目的是为了确保默认状态下不会执行意料以外的代码，
        /// 因此，如果需要在异步方法中调用同名的同步方法，请手动将此参数设置为SelfSynicMethod。</para>
        /// </summary>
        public DefaultMessageHandlerAsyncEvent DefaultMessageHandlerAsyncEvent
        {
            get { return _defaultMessageHandlerAsyncEvent; }
            set { _defaultMessageHandlerAsyncEvent = value; }
        }

        /// <summary>
        /// 是否同步向 MessageContext 写入 ResponseMessage，默认为否，将使用队列写入，提升响应速度
        /// </summary>
        public bool RecordResponseMessageSync { get; set; }

        public virtual async Task OnExecutingAsync(CancellationToken cancellationToken)
        {
           
        }

        public virtual async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (CancelExcute)
            {
                return;
            }

            //消息去重
            CheckMessageRepeat();

            if (CancelExcute)
            {
                return;
            }

            //进行 APM 记录
            ExecuteStatTime = SystemTime.Now;

            DataOperation apm = new DataOperation(PostModel?.DomainId);

            await apm.SetAsync(NeuCharApmKind.Message_Request.ToString(), 1, tempStorage: OpenId).ConfigureAwait(false);//Redis延迟：<1ms（约，测试数据，下同）
           
            await OnExecutingAsync(cancellationToken).ConfigureAwait(false);//Redis延迟：130ms（Demo示例中有需要使用缓存的逻辑代码）

            if (CancelExcute)
            {
                return;
            }

            try
            {
                if (RequestMessage == null)
                {
                    return;
                }

                //处理并设置 ResponseMessage
                await BuildResponseMessageAsync(cancellationToken).ConfigureAwait(false);//Redis延迟：230ms

                //记录上下文
                //此处修改
                if (MessageContextGlobalConfig.UseMessageContext && ResponseMessage != null && !string.IsNullOrEmpty(ResponseMessage.FromUserName))
                {
                    if (RecordResponseMessageSync)
                    {
                        await GlobalMessageContext.InsertMessageAsync(ResponseMessage);//耗时约：100ms（如使用队列，则不占用当前线程时间）
                    }
                    else
                    {
                        //回复消息记录可以使用队列，对时间不敏感
                        SenparcMessageQueue queue = new SenparcMessageQueue();
                        var lockKey = this.GetInsertMessageKey();
                        queue.Add($"{lockKey}{SystemTime.NowTicks}", async () =>
                        {
                            //这里为了提高分布式缓存的速度，使用 Unsafe 方法，可能出现不同步的情况是：同一个用户高频次访问（且不满足去重条件），
                            //第二个请求在构造函数中获得锁并且插入了新的 RequestMessage，但是此时第一个请求的 UnSafe 信息中还没有第二个请求的信息。
                            //由于概率较低，因此为了效率暂时忽略此情况，如果此情况极易发生，此处应改用 GetCurrentMessageContext()！    

                            //加同步锁
                            var cache = CacheStrategyFactory.GetObjectCacheStrategyInstance();
                            using (await cache.BeginCacheLockAsync(MessageContextGlobalConfig.MESSAGE_INSERT_LOCK_NAME, lockKey
                                       /*, 25, TimeSpan.FromMilliseconds(200)*/ /* 最多等待 5 秒钟*/))
                            {
                                await GlobalMessageContext.InsertMessageAsync(ResponseMessage);//耗时约：100ms（使用队列不占用当前线程时间）
                            }
                        });
                    }
                }

                await apm.SetAsync(NeuCharApmKind.Message_SuccessResponse.ToString(), 1, tempStorage: OpenId).ConfigureAwait(false);//Redis延迟：1ms
            }
            catch (Exception ex)
            {
                await apm.SetAsync(NeuCharApmKind.Message_Exception.ToString(), 1, tempStorage: OpenId).ConfigureAwait(false);
                throw new MessageHandlerException("MessageHandler中Execute()过程发生错误：" + ex.Message, ex);
            }
            finally
            {
                await OnExecutedAsync(cancellationToken).ConfigureAwait(false);//Redis延迟：3ms（Demo示例中有需要使用缓存的逻辑代码，已使用）

                await apm.SetAsync(NeuCharApmKind.Message_ResponseMillisecond.ToString(),
                    (SystemTime.Now - this.ExecuteStatTime).TotalMilliseconds, tempStorage: OpenId).ConfigureAwait(false);//Redis延迟：<1ms
            }

        }

        public abstract Task BuildResponseMessageAsync(CancellationToken cancellationToken);


        public virtual async Task OnExecutedAsync(CancellationToken cancellationToken)
        {

        }

        #endregion
    }
}
