using Senparc.CO2NET.Extensions;
using Senparc.CO2NET.Trace;
using Senparc.NeuChar;
using Senparc.NeuChar.Agents;
using Senparc.NeuChar.ApiHandlers;
using Senparc.NeuChar.Context;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Helpers;
using Senparc.NeuChar.MessageHandlers;
using Senparc.NeuChar.NeuralSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Senparc.NeuChar.NeuralSystems
{

    /// <summary>
    /// MessageHandler 的神经节点
    /// </summary>
    public partial class MessageHandlerNode : BaseNeuralNode
    {
        public override string Version { get; set; }

        /// <summary>
        /// 素材数据库
        /// </summary>
        public MaterialData MaterialData { get; set; }

        /// <summary>
        /// 设置信息（系统约定Config为固定名称）
        /// </summary>
        new public MessageReply Config { get; set; }

        /// <summary>
        /// MessageHandlerNode 构造函数
        /// </summary>
        public MessageHandlerNode()
        {
            Config = new MessageReply();
        }

        /// <summary>
        /// 执行NeuChar判断过程，获取响应消息
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="messageHandler"></param>
        /// <param name="accessTokenOrApi"></param>
        /// <returns></returns>
        public IResponseMessageBase Execute<TC, TRequest, TResponse>(IRequestMessageBase requestMessage, IMessageHandlerWithContext<TC, TRequest, TResponse> messageHandler,
            string accessTokenOrApi)
        where TC : class, IMessageContext<TRequest, TResponse>, new()
        where TRequest : IRequestMessageBase
        where TResponse : IResponseMessageBase
        {
            //if (accessTokenOrApi == null)
            //{
            //    throw new ArgumentNullException(nameof(accessTokenOrApi));
            //}
            var messageHandlerEnlightener = messageHandler.MessageEntityEnlightener;
            var appDataNode = messageHandler.CurrentAppDataNode;

            IResponseMessageBase responseMessage = null;

            //SenparcTrace.SendCustomLog("neuchar trace", "3");

            //进行APP特殊处理

            //判断状态
            var context = messageHandler.CurrentMessageContext;
            AppDataItem currentAppDataItem = null;

            switch (context.AppStoreState)
            {
                case AppStoreState.None://未进入任何应用
                case AppStoreState.Exit:
                    currentAppDataItem = null;
                    break;
                case AppStoreState.Enter:
                    //判断是否已过期
                    if (context.CurrentAppDataItem != null)
                    {
                        if (context.LastActiveTime.HasValue && context.LastActiveTime.Value.AddMinutes(context.CurrentAppDataItem.MessageKeepTime) < SystemTime.Now)
                        {
                            //没有上一条活动，或者对话已过期，则设置为退出状态
                            context.AppStoreState = AppStoreState.None;
                            context.CurrentAppDataItem = null;//退出清空
                        }
                        else
                        {
                            //继续保持应用状态
                            currentAppDataItem = context.CurrentAppDataItem;

                            if (requestMessage is IRequestMessageText || requestMessage is IRequestMessageEventKey requestClick)
                            {
                                var content = (requestMessage is IRequestMessageText requestText) ? requestText.Content : (requestMessage as IRequestMessageEventKey).EventKey;
                                if (!context.CurrentAppDataItem.MessageExitWord.IsNullOrEmpty() && context.CurrentAppDataItem.MessageExitWord.Equals(content, StringComparison.OrdinalIgnoreCase))
                                {
                                    //执行退出命令
                                    context.AppStoreState = AppStoreState.None;
                                    context.CurrentAppDataItem = null;//退出清空
                                    //currentAppDataItem = context.CurrentAppDataItem;//当前消息仍然转发（最后一条退出消息）
                                }
                            }
                        }
                    }
                    else
                    {
                        //已经进入App状态，但是没有标记退出，此处强制退出
                        context.AppStoreState = AppStoreState.None;
                    }
                    break;
                default:
                    break;
            }

            //TODO:暂时限定类型

            if (currentAppDataItem == null)
            {
                if (requestMessage is IRequestMessageText || requestMessage is IRequestMessageEventKey requestClick)
                {
                    var content = (requestMessage is IRequestMessageText requestText) ? requestText.Content : (requestMessage as IRequestMessageEventKey).EventKey;

                    currentAppDataItem = appDataNode.Config.AppDataItems
                        .FirstOrDefault(z => z.ExpireDateTime > SystemTime.Now && !z.MessageEnterWord.IsNullOrEmpty() && z.MessageEnterWord.Equals(content, StringComparison.OrdinalIgnoreCase));

                    if (currentAppDataItem != null && currentAppDataItem.MessageKeepTime > 0)
                    {
                        //初次进入应用
                        context.AppStoreState = AppStoreState.Enter;
                        context.CurrentAppDataItem = currentAppDataItem;
                    }
                }
            }

            if (currentAppDataItem != null) //已经锁定某个App
            {
                //NeuralSystem.Instance.NeuCharDomainName = "https://www.neuchar.com";

                //转发AppData消息
                var neuCharUrl = $"{NeuralSystem.Instance.NeuCharDomainName}/App/Weixin?appId={currentAppDataItem.Id}&neuralAppId={appDataNode.NeuralAppId}";
                try
                {
                    responseMessage = MessageAgent.RequestResponseMessage(messageHandler, neuCharUrl, "Senparc", requestMessage.ConvertEntityToXmlString());
                }
                catch (Exception ex)
                {
                    Senparc.CO2NET.Trace.SenparcTrace.SendCustomLog("NeuChar 远程调用 APP 失败", ex.Message);
                }
            }


            //APP特殊处理结束


            if (responseMessage != null)
            {
                if (messageHandler.MessageEntityEnlightener.PlatformType == PlatformType.WeChat_MiniProgram && responseMessage is IResponseMessageText)
                {
                    //小程序
                    messageHandler.ApiEnlightener.SendText(accessTokenOrApi, messageHandler.WeixinOpenId, (responseMessage as IResponseMessageText).Content);
                    return new SuccessResponseMessage();
                }
                else
                {
                    return responseMessage;
                }
            }

            //处理普通消息回复
            switch (requestMessage.MsgType)
            {
                case RequestMsgType.Text:
                    {
                        try
                        {
                            //SenparcTrace.SendCustomLog("neuchar trace", "3.1");

                            var textRequestMessage = requestMessage as IRequestMessageText;

                            //遍历所有的消息设置
                            foreach (var messagePair in Config.MessagePair.Where(z => z.Request.Type == RequestMsgType.Text))
                            {
                                //遍历每一个消息设置中的关键词
                                var pairSuccess = messagePair.Request.Keywords.Exists(keyword => keyword.Equals(textRequestMessage.Content, StringComparison.OrdinalIgnoreCase));
                                if (pairSuccess)
                                {
                                    responseMessage = GetResponseMessage(requestMessage, messagePair.Responses, messageHandler, accessTokenOrApi);
                                }

                                if (responseMessage != null)
                                {
                                    break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            SenparcTrace.SendCustomLog("neuchar text error", ex.Message + "\r\n|||\r\n" + (ex.InnerException != null ? ex.InnerException.ToString() : ""));
                        }

                    }
                    break;
                case RequestMsgType.Image:
                    {
                        var imageRequestMessage = requestMessage as IRequestMessageImage;
                        //遍历所有的消息设置

                        foreach (var messagePair in Config.MessagePair.Where(z => z.Request.Type == RequestMsgType.Image))
                        {
                            responseMessage = GetResponseMessage(requestMessage, messagePair.Responses, messageHandler, accessTokenOrApi);

                            if (responseMessage != null)
                            {
                                break;
                            }
                        }
                    }
                    break;
                case RequestMsgType.Event:
                    {
                        //菜单或其他系统事件
                        if (requestMessage is IRequestMessageEvent eventRequestMessage)
                        {
                            var eventType = eventRequestMessage.EventName.ToUpper();

                            //构造返回结果
                            List<Response> responses = new List<Response>();


                            switch (eventType)
                            {
                                case "CLICK" when requestMessage is IRequestMessageEventKey clickRequestMessage:
                                    {
                                        //TODO:暂时只支持CLICK，因此在这里遍历
                                        foreach (var messagePair in Config.MessagePair.Where(z => z.Request.Type == RequestMsgType.Event))
                                        {
                                            var pairSuccess = messagePair.Request.Keywords.Exists(keyword => keyword.Equals(clickRequestMessage.EventKey, StringComparison.OrdinalIgnoreCase));
                                            if (pairSuccess)
                                            {
                                                try
                                                {
                                                    responseMessage = GetResponseMessage(requestMessage, messagePair.Responses, messageHandler, accessTokenOrApi);

                                                }
                                                catch (Exception ex)
                                                {
                                                    SenparcTrace.SendCustomLog("CLICK 跟踪 1.1", ex.Message + "\r\n" + ex.StackTrace);
                                                }
                                            }

                                            if (responseMessage != null)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                        {
                            //下级模块中没有正确处理 requestMessage 类型
                        }
                    }
                    break;
                default:
                    //不作处理

                    //throw new UnknownRequestMsgTypeException("NeuChar未支持的的MsgType请求类型："+ requestMessage.MsgType, null);
                    break;

            }
            //SenparcTrace.SendCustomLog("neuchar trace", "4");

            return responseMessage;
        }

#if !NET35 && !NET40
        /// <summary>
        /// 执行NeuChar判断过程，获取响应消息
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<IResponseMessageBase> ExecuteAsync<TC, TRequest, TResponse>(IRequestMessageBase requestMessage, IMessageHandlerWithContext<TC, TRequest, TResponse> messageHandler,
            string accessTokenOrApi)
        where TC : class, IMessageContext<TRequest, TResponse>, new()
        where TRequest : IRequestMessageBase
        where TResponse : IResponseMessageBase
        {
            //SenparcTrace.SendCustomLog("neuchar trace","1");
            return await Task.Run(() => Execute(requestMessage, messageHandler, accessTokenOrApi));
        }
#endif
    }

}
