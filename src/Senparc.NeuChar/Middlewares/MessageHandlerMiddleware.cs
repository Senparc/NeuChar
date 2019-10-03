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
    /// <typeparam name="TMC"></typeparam>
    /// <typeparam name="TPM"></typeparam>
    /// <typeparam name="TS"></typeparam>
    public abstract class MessageHandlerMiddleware<TMC, TPM, TS>
        where TMC : class, IMessageContext<IRequestMessageBase, IResponseMessageBase>, new()
        where TPM : IEncryptPostModel
        where TS : class
    {
        private readonly RequestDelegate _next;
        private readonly Func<Stream, TPM, int, MessageHandler<TMC, IRequestMessageBase, IResponseMessageBase>> _messageHandlerFunc;
        private readonly Func<HttpContext, TS> _senparcWeixinSettingFunc;
        private readonly MessageHandlerMiddlewareOptions<TS> _options;

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

            if (_options.SettingFunc == null)
            {
                throw new MessageHandlerException($"{nameof(options)} 中必须对 SenparcWeixinSetting 进行配置！");
            }

            _senparcWeixinSettingFunc = _options.SettingFunc;
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
            var senparcWeixinSetting = _senparcWeixinSettingFunc(context);

            TPM postModel = GetPostModel(context);
            string echostr = GetEchostr(context);

            // GET 验证
            if (context.Request.Method.ToUpper() == "GET")
            {
                if (!await GetCheckSignature(context).ConfigureAwait(false))
                {
                    return;
                }
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

                context.Response.ContentType = $"text/{(isXml ? "xml":"plain")};charset=utf-8";
                await context.Response.WriteAsync(returnResult);
            }

            //不再继续向下执行
            //await _next(context).ConfigureAwait(false);
        }

    }
}

#endif