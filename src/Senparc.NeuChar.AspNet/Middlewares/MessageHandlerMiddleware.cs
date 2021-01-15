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
    
    文件名：MessageHandlerMiddleware.cs
    文件功能描述：MessageHandler 中间件基类
    
    
    创建标识：Senparc - 20191003
   
    修改标识：Senparc - 20191005
    修改描述：提供 ExecuteCancellationTokenSource 属性
   
----------------------------------------------------------------*/

#if NETSTANDARD2_0 || NETSTANDARD2_1
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Senparc.CO2NET.AspNet.HttpUtility;
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
    #region 接口

    /// <summary>
    /// MessageHandler 中间件基类接口 TODO：独立到文件
    /// </summary>
    /// <typeparam name="TMC"></typeparam>
    /// <typeparam name="TPM"></typeparam>
    /// <typeparam name="TS"></typeparam>
    public interface IMessageHandlerMiddleware<TMC, TPM, TS> : IMessageHandlerMiddleware<TMC, IRequestMessageBase, IResponseMessageBase, TPM, TS>
         where TMC : class, IMessageContext<IRequestMessageBase, IResponseMessageBase>, new()
        where TPM : IEncryptPostModel
    { }

    /// <summary>
    /// MessageHandler 中间件基类接口 TODO：独立到文件
    /// </summary>
    /// <typeparam name="TMC"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <typeparam name="TPM"></typeparam>
    /// <typeparam name="TS"></typeparam>
    public interface IMessageHandlerMiddleware<TMC, TRequest, TResponse, TPM, TS>
        where TMC : class, IMessageContext<TRequest, TResponse>, new()
        where TRequest : class, IRequestMessageBase
        where TResponse : class, IResponseMessageBase
        where TPM : IEncryptPostModel
    {
        /// <summary>
        /// 执行 MessageHandler.ExecuteAsync() 时提供 CancellationTokenSource.CancellationToken
        /// </summary>
        abstract CancellationTokenSource ExecuteCancellationTokenSource { get; }

        /// <summary>
        /// 生成 PostModel
        /// </summary>
        /// <returns></returns>
        abstract TPM GetPostModel(HttpContext context);

        /// <summary>
        /// 获取 echostr（如果有）
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        abstract string GetEchostr(HttpContext context);

        /// <summary>
        /// GET 请求下的签名验证
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        abstract Task<bool> GetCheckSignature(HttpContext context);

        /// <summary>
        /// POST 请求下的签名验证
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        abstract Task<bool> PostCheckSignature(HttpContext context);

        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Task Invoke(HttpContext context);

        /// <summary>
        /// 获取 GET 请求时错误响应信息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="supportSignature">提供的签名</param>
        /// <param name="correctSignature">正确的签名</param>
        /// <returns></returns>
        string GetGetCheckFaildMessage(HttpContext context, string supportSignature, string correctSignature);
    }

    #endregion

    #region 实现

    /// <summary>
    /// MessageHandler 中间件基类
    /// </summary>
    /// <typeparam name="TMC">上下文</typeparam>
    /// <typeparam name="TPM">PostModel</typeparam>
    /// <typeparam name="TS">Setting 类，如 SenparcWeixinSetting</typeparam>
    public abstract class MessageHandlerMiddleware<TMC, TPM, TS> : MessageHandlerMiddleware<TMC, IRequestMessageBase, IResponseMessageBase, TPM, TS>
        , IMessageHandlerMiddleware<TMC, TPM, TS>
        where TMC : class, IMessageContext<IRequestMessageBase, IResponseMessageBase>, new()
        where TPM : IEncryptPostModel
    {
        public MessageHandlerMiddleware(RequestDelegate next, Func<Stream, TPM, int, MessageHandler<TMC, IRequestMessageBase, IResponseMessageBase>> messageHandler, Action<MessageHandlerMiddlewareOptions<TS>> options)
            : base(next, messageHandler, options)
        {
        }
    }

    /// <summary>
    /// MessageHandler 中间件基类
    /// </summary>
    /// <typeparam name="TMC">上下文</typeparam>
    /// <typeparam name="TPM">PostModel</typeparam>
    /// <typeparam name="TS">Setting 类，如 SenparcWeixinSetting</typeparam>
    public abstract class MessageHandlerMiddleware<TMC, TRequest, TResponse, TPM, TS>
        : IMessageHandlerMiddleware<TMC, TRequest, TResponse, TPM, TS>
        where TMC : class, IMessageContext<TRequest, TResponse>, new()
        where TRequest : class, IRequestMessageBase
        where TResponse : class, IResponseMessageBase
        where TPM : IEncryptPostModel
    {
        protected readonly RequestDelegate _next;
        protected readonly Func<Stream, TPM, int, MessageHandler<TMC, TRequest, TResponse>> _messageHandlerFunc;
        protected readonly Func<HttpContext, TS> _accountSettingFunc;
        protected readonly MessageHandlerMiddlewareOptions<TS> _options;
        protected CancellationTokenSource _executeCancellationTokenSource;


        /// <summary>
        /// 执行 MessageHandler.ExecuteAsync() 时提供 CancellationTokenSource.CancellationToken
        /// </summary>
        public CancellationTokenSource ExecuteCancellationTokenSource
        {
            get
            {
                if (_executeCancellationTokenSource == null)
                {
                    _executeCancellationTokenSource = new CancellationTokenSource();
                }
                return _executeCancellationTokenSource;
            }
        }

        /// <summary>
        /// EnableRequestRewindMiddleware
        /// </summary>
        /// <param name="next"></param>
        public MessageHandlerMiddleware(RequestDelegate next,
            Func<Stream, TPM, int, MessageHandler<TMC, TRequest, TResponse>> messageHandler,
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

        /// <summary>
        /// GET 请求下的签名验证
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract Task<bool> GetCheckSignature(HttpContext context);

        /// <summary>
        /// POST 请求下的签名验证
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public abstract Task<bool> PostCheckSignature(HttpContext context);


        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            MessageHandler<TMC, TRequest, TResponse> messageHandler = null;
            try
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


                    messageHandler = _messageHandlerFunc(context.Request.GetRequestMemoryStream(), postModel, _options.MaxRecordCount);

                    messageHandler.DefaultMessageHandlerAsyncEvent = _options.DefaultMessageHandlerAsyncEvent;

                    #region 设置消息去重 设置

                    /* 如果需要添加消息去重功能，只需打开OmitRepeatedMessage功能，SDK会自动处理。
                     * 收到重复消息通常是因为微信服务器没有及时收到响应，会持续发送2-5条不等的相同内容的RequestMessage*/
                    messageHandler.OmitRepeatedMessage = _options.OmitRepeatedMessage;//默认已经开启，此处仅作为演示，也可以设置为false在本次请求中停用此功能

                    #endregion

                    if (_options.EnableRequestLog)
                    {
                        messageHandler.SaveRequestMessageLog();//记录 Request 日志（可选）
                    }

                    await messageHandler.ExecuteAsync(ExecuteCancellationTokenSource.Token); //执行微信处理过程（关键）

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
            }
            catch (Exception ex)
            {
                var msg = $@"中间件类型：{this.GetType().Name}
MessageHandler 类型：{(messageHandler == null ? "尚未生成" : messageHandler.GetType().Name)}
异常信息：{ex.ToString()}";

                SenparcTrace.SendCustomLog($"MessageHandlerware 过程发证异常", msg);

                //使用外部的委托对异常过程进行处理
                if (_options.AggregateExceptionCatch != null)
                {
                    var aggregateException = new AggregateException(ex);
                    var continueRun = _options.AggregateExceptionCatch(aggregateException);
                    if (!continueRun)
                    {
                        throw;
                    }
                }
            }
            //不再继续向下执行
            //await _next(context).ConfigureAwait(false);
        }


        /// <summary>
        /// 获取 GET 请求时错误响应信息
        /// </summary>
        /// <param name="context"></param>
        /// <param name="supportSignature">提供的签名</param>
        /// <param name="correctSignature">正确的签名</param>
        /// <returns></returns>
        public string GetGetCheckFaildMessage(HttpContext context, string supportSignature, string correctSignature)
        {
            var postModel = GetPostModel(context);
            var banMsg = "javascript:alert('出于安全考虑，请在服务器本地打开此页面，查看链接')";

            var isLocal = context.Request.IsLocal();
            string signature = isLocal
                        ? $@"提供签名：{supportSignature.HtmlEncode()}<br />
正确签名：{correctSignature.HtmlEncode()}<br />
<br />
<!--校验结果：<strong style=""color:red"">{(postModel.Signature == correctSignature ? "成功" : "失败")}</strong><br />
<br />-->
<span style=""word-wrap:break-word"">Url：{context.Request.PathAndQuery().HtmlEncode()}</span>"
                        : "出于安全考虑，系统不能远程传输签名信息，请在服务器本地打开此页面，查看信息！";
            string seeDetail = isLocal ? "https://www.cnblogs.com/szw/p/token-error.html" : banMsg;
            string openSimulateTool = isLocal ? "https://sdk.weixin.senparc.com/SimulateTool" : banMsg;
            string targetBlank = isLocal ? @"target=""_balank""" : "";

            return $@"<div style=""width:600px; margin:50px auto; padding:20px 50px 20px 50px; border:#9ed900 3px solid; background:#f0fcff;border-radius:15px; box-shadow: 0 25.6px 57.6px rgba(0,0,0,.22), 0px 7px 14.4px rgba(96, 134, 93, 0.97);"">
<h1>此 Url 可用于服务器 token 签名校验<h1>
<h2>签名信息</h2>
{signature}<br /><br />
<h2>提示</h2>
<ol>
<li>如果你在浏览器中打开并看到此提示，那么意味着此地址可以被作为微信公众账号后台的 Url，并可以开始进行官方的对接校验，请注意保持 Token 设置的一致。</li>
<li>看到此提示，证明本系统对微信消息进行了符合官方要求的安全验证。</li>
<li>特别说明：以上签名错误信息只对当前提供的参数进行签名安全检验，<span style=""color:#f00"">无法</span>用于验证系统其他功能正常与否。</li>
</ol>
<p><a href=""{seeDetail}"" {targetBlank}>查看详情</a> | <a href=""{openSimulateTool}"" {targetBlank}>使用消息模拟器测试</a></p>
<p style=""text-align: right; color: #aaaa;"">{SystemTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}</p>
</div>";
        }

    }

    #endregion

    #region 扩展方法

    /// <summary>
    /// MessageHandlerMiddleware 扩展类
    /// </summary>
    public static class MessageHandlerMiddlewareExtension
    {
        /// <summary>
        /// 使用 MessageHandler 配置。注意：会默认使用异步方法 messageHandler.ExecuteAsync()。
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="pathMatch">路径规则（路径开头，可带参数）</param>
        /// <param name="messageHandler">MessageHandler</param>
        /// <param name="options">设置选项</param>
        /// <returns></returns>
        public static IApplicationBuilder UseMessageHandler<TMHM, TMC, TPM, TS>(this IApplicationBuilder builder,
            PathString pathMatch, Func<Stream, TPM, int, MessageHandler<TMC, IRequestMessageBase, IResponseMessageBase>> messageHandler, Action<MessageHandlerMiddlewareOptions<TS>> options)
                where TMHM : IMessageHandlerMiddleware<TMC, IRequestMessageBase, IResponseMessageBase, TPM, TS>
                where TMC : class, IMessageContext<IRequestMessageBase, IResponseMessageBase>, new()
                where TPM : IEncryptPostModel
            //where TS : class
        {
            return UseMessageHandler<TMHM, IRequestMessageBase, IResponseMessageBase, TMC, TPM, TS>(builder, pathMatch, messageHandler, options);
        }

        /// <summary>
        /// 使用 MessageHandler 配置。注意：会默认使用异步方法 messageHandler.ExecuteAsync()。
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="pathMatch">路径规则（路径开头，可带参数）</param>
        /// <param name="messageHandler">MessageHandler</param>
        /// <param name="options">设置选项</param>
        /// <returns></returns>
        public static IApplicationBuilder UseMessageHandler<TMHM, TRequest, TResponse, TMC, TPM, TS>(this IApplicationBuilder builder,
            PathString pathMatch, Func<Stream, TPM, int, MessageHandler<TMC, TRequest, TResponse>> messageHandler, Action<MessageHandlerMiddlewareOptions<TS>> options)
                where TMHM : IMessageHandlerMiddleware<TMC, TRequest, TResponse, TPM, TS>
                where TRequest : class, IRequestMessageBase
                where TResponse : class, IResponseMessageBase
                where TMC : class, IMessageContext<TRequest, TResponse>, new()
                where TPM : IEncryptPostModel
            //where TS : class
        {
            return builder.Map(pathMatch, app =>
            {
                app.UseMiddleware<TMHM>(messageHandler, options);
            });
        }
    }


    #endregion
}

#endif