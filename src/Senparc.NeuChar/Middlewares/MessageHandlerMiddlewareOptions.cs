#region Apache License Version 2.0
/*----------------------------------------------------------------

Copyright 2019 Suzhou Senparc Network Technology Co.,Ltd.

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
    Copyright (C) 2019 Senparc
    
    文件名：MessageHandlerMiddlewareOptions.cs
    文件功能描述：MessageHandler 中间件选项设置类配置信息
    
    
    创建标识：Senparc - 20191003
    
----------------------------------------------------------------*/

#if NETSTANDARD2_0 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETCOREAPP3_0

using Microsoft.AspNetCore.Http;
using Senparc.NeuChar.MessageHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senparc.NeuChar.Middlewares
{
    /// <summary>
    /// MessageHandlerMiddleware 配置信息
    /// </summary>
    /// <typeparam name="T">AccountSetting 类型，如公众号的 SenparcWeixinSetting</typeparam>
    public class MessageHandlerMiddlewareOptions<T>
       //where T : class
    {
        /// <summary>
        /// 启用 RequestMessage 的日志记录
        /// </summary>
        public bool EnableRequestLog { get; set; } = true;

        /// <summary>
        /// 启用 ResponseMessage 的日志记录
        /// </summary>
        public bool EnbleResponseLog { get; set; } = true;

        /// <summary>
        /// 在没有 override 的情况下，MessageHandler 事件异步方法的默认调用方法
        /// </summary>
        public DefaultMessageHandlerAsyncEvent DefaultMessageHandlerAsyncEvent { get; set; } = DefaultMessageHandlerAsyncEvent.DefaultResponseMessageAsync;

        /// <summary>
        /// 上下文最大纪录数量（默认为 10)
        /// </summary>
        public int MaxRecordCount { get; set; } = 10;

        /// <summary>
        /// 是否去重（默认为 true）
        /// </summary>
        public bool OmitRepeatedMessage { get; set; } = true;

        /// <summary>
        /// 如公众号的 SenparcWeixinSetting 信息，必须包含 Token、AppId，以及 EncodingAESKey（如果有）
        /// </summary>
        public Func<HttpContext, T> AccountSettingFunc { get; set; }
    }
}

#endif