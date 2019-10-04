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
    
    文件名：MessageHandlerMiddleware.cs
    文件功能描述：MessageHandler 中间件基类
    
    
    创建标识：Senparc - 20191003
    
----------------------------------------------------------------*/

#if NETSTANDARD2_0 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETCOREAPP3_0
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Senparc.CO2NET.Extensions;
using Senparc.CO2NET.HttpUtility;
using Senparc.CO2NET.Trace;
using Senparc.NeuChar.Context;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Exceptions;
using Senparc.NeuChar.MessageHandlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Senparc.NeuChar.Middlewares
{
    /// <summary>
    /// MessageHandler 中间件基类
    /// </summary>
    /// <typeparam name="TMC">上下文</typeparam>
    /// <typeparam name="TPM">PostModel</typeparam>
    /// <typeparam name="TS">Setting 类，如 SenparcWeixinSetting</typeparam>
    public abstract class MessageHandlerMiddleware<TMC, TPM, TS>
        where TMC : class, IMessageContext<IRequestMessageBase, IResponseMessageBase>, new()
        where TPM : IEncryptPostModel
        where TS : class
    {
        protected readonly RequestDelegate _next;
        protected readonly Func<Stream, TPM, int, MessageHandler<TMC, IRequestMessageBase, IResponseMessageBase>> _messageHandlerFunc;
        protected readonly Func<HttpContext, TS> _accountSettingFunc;
        protected readonly MessageHandlerMiddlewareOptions<TS> _options;

        /// <summary>
        /// 获取 GET 请求时错误响应信息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="currectSignature"></param>
        /// <returns></returns>
        public string GetGetCheckFaildMessage(HttpContext context, string currectSignature)
        {
            var postModel = GetPostModel(context);

            string signature = context.Request.IsLocal()
                        ? $"提供签名：{postModel.Signature}<br />正确签名：{currectSignature}"
                        : "";
            string seeDetail = context.Request.IsLocal()
                        ? "https://www.cnblogs.com/szw/p/token-error.html"
                        : "javascript:alert('出于安全考虑，请在服务器本地打开此页面，查看链接')";

            return $@"<div style=""width:600px; margin:auto 20px; padding:50px; border:#9ed900 3px solid; background:#f0fcff; border:border-radius:5px;"">
服务器 token 签名校验失败！<br>
<h2>签名信息</h2>
{signature}<br /><br />
<h2>提示</h2>
如果你在浏览器中打开并看到这句话，那么看到这条消息<span style=""color:#f00"">并不能说明</span>你的程序有问题，
而是意味着此地址可以被作为微信公众账号后台的 Url，并开始进行官方的对接校验，请注意保持 Token 设置的一致。<br /><br />

<a href=""{seeDetail}"" target=""_balank"">查看详情</a>
</div>";
        }

        /// <summary>
        /// EnableRequestRewindMiddleware
        /// </summary>
        /// <param name="next"></param>
        public MessageHandlerMiddleware(RequestDelegate next, Func<Stream, TPM, int, MessageHandler<TMC, IRequestMessageBase, IResponseMessageBase>> messageHandler,
            Action<MessageHandlerMiddlewareOptions<TS>> options)
        {
            _next = next;
            _messageHandlerFunc = messageHandler;

            if (options == null)
            {
                throw new MessageHandlerException($"{nameof(options)} 参数必须提供！");
            }

            _options = new MessageHandlerMiddlewareOptions<TS>();//生成一个新的 Option 对象
            options(_options);//设置 Opetion

            if (_options.AccountSettingFunc == null)
            {
                throw new MessageHandlerException($"{nameof(options)} 中必须对 SenparcWeixinSetting 进行配置！");
            }

            _accountSettingFunc = _options.AccountSettingFunc;
        }

        /// <summary>
        /// 生成 PostModel
        /// </summary>
        /// <returns></returns>
        public abstract TPM GetPostModel(HttpContext context);

        /// <summary>
        /// 获取 echostr（如果有）
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract string GetEchostr(HttpContext context);

        public abstract Task<bool> GetCheckSignature(HttpContext context);

        public abstract Task<bool> PostCheckSignature(HttpContext context);


        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            var senparcWeixinSetting = _accountSettingFunc(context);

            TPM postModel = GetPostModel(context);
            string echostr = GetEchostr(context);

            // GET 验证
            if (context.Request.Method.ToUpper() == "GET")
            {
                await GetCheckSignature(context).ConfigureAwait(false);
                return;
            }
            // POST 消息请求
            else if (context.Request.Method.ToUpper() == "POST")
            {
                if (!await PostCheckSignature(context).ConfigureAwait(false))
                {
                    return;
                }

                var cancellationToken = new CancellationToken();//给异步方法使用

                var messageHandler = _messageHandlerFunc(context.Request.GetRequestMemoryStream(), postModel, _options.MaxRecordCount);


                #region 没有重写的异步方法将默认尝试调用同步方法中的代码（为了偷懒）

                /* 使用 SelfSynicMethod 的好处是可以让异步、同步方法共享同一套（同步）代码，无需写两次，
                 * 当然，这并不一定适用于所有场景，所以是否选用需要根据实际情况而定，这里只是演示，并不盲目推荐。*/
                messageHandler.DefaultMessageHandlerAsyncEvent = _options.DefaultMessageHandlerAsyncEvent;

                #endregion

                #region 设置消息去重 设置

                /* 如果需要添加消息去重功能，只需打开OmitRepeatedMessage功能，SDK会自动处理。
                 * 收到重复消息通常是因为微信服务器没有及时收到响应，会持续发送2-5条不等的相同内容的RequestMessage*/
                messageHandler.OmitRepeatedMessage = _options.OmitRepeatedMessage;//默认已经开启，此处仅作为演示，也可以设置为false在本次请求中停用此功能

                #endregion

                if (_options.EnableRequestLog)
                {
                    messageHandler.SaveRequestMessageLog();//记录 Request 日志（可选）
                }

                await messageHandler.ExecuteAsync(cancellationToken); //执行微信处理过程（关键）

                if (_options.EnbleResponseLog)
                {
                    messageHandler.SaveResponseMessageLog();//记录 Response 日志（可选）
                }

                string returnResult = null;
                bool isXml = false;
                //使用IMessageHandler输出
                if (messageHandler is IMessageHandlerDocument messageHandlerDocument)
                {
                    //先从 messageHandlerDocument.TextResponseMessage 中取值
                    returnResult = messageHandlerDocument.TextResponseMessage?.Replace("\r\n", "\n");

                    if (returnResult == null)
                    {
                        var finalResponseDocument = messageHandlerDocument.FinalResponseDocument;

                        if (finalResponseDocument != null)
                        {
                            returnResult = finalResponseDocument.ToString()?.Replace("\r\n", "\n");
                            isXml = true;
                        }
                        else
                        {
                            //throw new Senparc.Weixin.MP.WeixinException("FinalResponseDocument不能为Null！", null);
                        }
                    }
                }
                else
                {
                    throw new MiddlewareException("IMessageHandlerDocument 类型的 MessageHandler 不能为 Null！", null);
                }

                returnResult = returnResult ?? "";

                context.Response.ContentType = $"text/{(isXml ? "xml" : "plain")};charset=utf-8";
                await context.Response.WriteAsync(returnResult);
            }

            //不再继续向下执行
            //await _next(context).ConfigureAwait(false);
        }

    }
}

#endif