#region Apache License Version 2.0
/*----------------------------------------------------------------

Copyright 2018 Suzhou Senparc Network Technology Co.,Ltd.

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
    Copyright (C) 2018 Senparc
    
    文件名：IMessageHandlerWithContext.cs
    文件功能描述：具有上下文的 MessageHandler 接口


    创建标识：Senparc - 20181022

----------------------------------------------------------------*/


/*
 * V3.2
 * V4.0 添加异步方法
 */

using Senparc.NeuChar.Context;
using Senparc.NeuChar.Entities;
using System;

namespace Senparc.NeuChar.MessageHandlers
{
    /// <summary>
    /// 具有上下文的 MessageHandler 接口
    /// </summary>
    /// <typeparam name="TC"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface IMessageHandlerWithContext<TC, TRequest, TResponse> : IMessageHandler<TRequest, TResponse>
        where TC : class, IMessageContext<TRequest, TResponse>, new()
        where TRequest : IRequestMessageBase
        where TResponse : IResponseMessageBase
    {
        /// <summary>
        /// 全局消息上下文
        /// </summary>
        [Obsolete("请使用 GlobalMessageContext")]
        GlobalMessageContext<TC, TRequest, TResponse> WeixinContext { get; }
        /// <summary>
        /// 全局消息上下文
        /// </summary>
        GlobalMessageContext<TC, TRequest, TResponse> GlobalMessageContext { get; }
        /// <summary>
        /// 当前用户消息上下文
        /// </summary>
        TC CurrentMessageContext { get; }
        /// <summary>
        /// 忽略重复发送的同一条消息（通常因为微信服务器没有收到及时的响应）
        /// </summary>
         bool OmitRepeatedMessage { get; set; }
        /// <summary>
        /// 消息是否已经被去重
        /// </summary>
         bool MessageIsRepeated { get; set; }
    }
}
