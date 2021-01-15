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
    
    文件名：RequestUtility.cs
    文件功能描述：微信请求集中处理接口
    
    
    创建标识：Senparc - 20150211
    
    修改标识：Senparc - 20150303
    修改描述：整理接口

    -- NeuChar --

    修改标识：Senparc - 20181022
    修改描述：添加 IMessageHandlerExtensionProperties 接口

    修改标识：Senparc - 2019104
    修改描述：改为以异步方法为主，删除 BuildResponseMessage() 同步方法

    修改标识：Senparc - 2019104
    修改描述：v1.0.102 MessageHandler 添加 OnlyAllowEncryptMessage 属性

----------------------------------------------------------------*/

/*
 * V4.0 添加异步方法
 * V6.1 添加 OnlyAllowEncryptMessage 属性
 */

using Senparc.NeuChar.ApiHandlers;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.NeuralSystems;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Senparc.NeuChar.MessageHandlers
{
    /// <summary>
    /// IMessageHandlerExtensionProperties 接口
    /// </summary>
    public interface IMessageHandlerBase : IMessageHandlerEnlightener, IMessageHandlerNeuralNodes
    {
        /// <summary>
        /// 发送者用户名（OpenId）
        /// </summary>
        string WeixinOpenId { get; }

        /// <summary>
        /// 取消执行Execute()方法。一般在OnExecuting()中用于临时阻止执行Execute()。
        /// 默认为False。
        /// 如果在执行OnExecuting()执行前设为True，则所有OnExecuting()、Execute()、OnExecuted()代码都不会被执行。
        /// 如果在执行OnExecuting()执行过程中设为True，则后续Execute()及OnExecuted()代码不会被执行。
        /// 建议在设为True的时候，给ResponseMessage赋值，以返回友好信息。
        /// </summary>
        bool CancelExcute { get; set; }


        /// <summary>
        /// 忽略重复发送的同一条消息（通常因为微信服务器没有收到及时的响应）
        /// </summary>
        bool OmitRepeatedMessage { get; set; }

        /// <summary>
        /// 消息是否已经被去重
        /// </summary>
        bool MessageIsRepeated { get; set; }

        /// <summary>
        /// 是否使用了MessageAgent代理
        /// </summary>
        bool UsedMessageAgent { get; set; }

        /// <summary>
        /// 是否使用了加密消息格式
        /// </summary>
        bool UsingEncryptMessage { get; set; }

        /// <summary>
        /// 是否使用了兼容模式加密信息
        /// </summary>
        bool UsingCompatibilityModelEncryptMessage { get; set; }

        /// <summary>
        /// 当平台同时兼容明文消息和加密消息时，只允许处理加密消息（不允许处理明文消息），默认为 False
        /// </summary>
        bool OnlyAllowEncryptMessage { get; set; }


        /// <summary>
        /// PostModel
        /// </summary>
        IEncryptPostModel PostModel { get; set; }

        /// <summary>
        /// ServiceProvide
        /// </summary>
        IServiceProvider ServiceProvider { get; set; }

        #region 同步方法

        /// <summary>
        /// 执行微信请求前触发
        /// </summary>
        void OnExecuting();

        /// <summary>
        /// 执行请求
        /// </summary>
        void Execute();

        /// <summary>
        /// 执行微信请求后触发
        /// </summary>
        void OnExecuted();

        #endregion


#if !NET35 && !NET40
        #region 异步方法

        /// <summary>
        /// 【异步方法】执行微信请求前触发
        /// </summary>
        Task OnExecutingAsync(CancellationToken cancellationToken);

        /// <summary>
        /// 【异步方法】执行微信请求
        /// </summary>
        Task ExecuteAsync(CancellationToken cancellationToken);
        /// <summary>
        /// 执行请求内部的消息整理逻辑
        /// </summary>
        Task BuildResponseMessageAsync(CancellationToken cancellationToken);

        /// <summary>
        /// 【异步方法】执行微信请求后触发
        /// </summary>
        Task OnExecutedAsync(CancellationToken cancellationToken);

        #endregion
#endif

    }

    /// <summary>
    /// IMessageHandler 接口
    /// </summary>
    /// <typeparam name="TRequest">IRequestMessageBase</typeparam>
    /// <typeparam name="TResponse">IResponseMessageBase</typeparam>
    public interface IMessageHandler<TRequest, TResponse> : IMessageHandlerDocument, IMessageHandlerBase
        where TRequest : IRequestMessageBase
        where TResponse : IResponseMessageBase
    {
        /// <summary>
        /// 请求实体
        /// </summary>
        TRequest RequestMessage { get; set; }
        /// <summary>
        /// 响应实体
        /// 只有当执行Execute()方法后才可能有值
        /// </summary>
        TResponse ResponseMessage { get; set; }

    }
}
