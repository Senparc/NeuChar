using Senparc.CO2NET.Extensions;
using Senparc.CO2NET.Trace;
using Senparc.NeuChar;
using Senparc.NeuChar.ApiHandlers;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Senparc.NeuChar.MessageHandlers
{
    /// <summary>
    /// MessageHandler 的神经节点
    /// </summary>
    public class MessageHandlerNode : BaseNeuralNode
    {
        public override string Version { get; set; }


        /// <summary>
        /// 设置信息（系统约定Config为固定名称）
        /// </summary>
        new public MessageReply Config { get; set; }

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
        public IResponseMessageBase Execute(IRequestMessageBase requestMessage, IMessageHandlerEnlightener messageHandler, string accessTokenOrApi)
        {
            //SenparcTrace.SendCustomLog("neuchar trace", "2");

            if (accessTokenOrApi == null)
            {
                throw new ArgumentNullException(nameof(accessTokenOrApi));
            }

            IResponseMessageBase responseMessage = null;

            //SenparcTrace.SendCustomLog("neuchar trace", "3");

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
                                foreach (var keyword in messagePair.Request.Keywords)
                                {
                                    if (keyword.Equals(textRequestMessage.Content, StringComparison.OrdinalIgnoreCase))//TODO:加入大小写敏感设计
                                    {
                                        responseMessage = GetResponseMessage(requestMessage, messagePair.Responses, messageHandler, accessTokenOrApi);
                                        break;
                                    }
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
        public async Task<IResponseMessageBase> ExecuteAsync(IRequestMessageBase requestMessage, IMessageHandlerEnlightener messageHandler, string accessTokenOrApi)
        {
            SenparcTrace.SendCustomLog("neuchar trace","1");
            return await Task.Run(() => Execute(requestMessage, messageHandler, accessTokenOrApi));
        }
#endif
        #region 返回信息

        /// <summary>
        /// 获取响应消息
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="responseConfigs"></param>
        /// <param name="messageHandler"></param>
        /// <param name="accessTokenOrApi"></param>
        /// <returns></returns>
        private IResponseMessageBase GetResponseMessage(IRequestMessageBase requestMessage, List<Response> responseConfigs, IMessageHandlerEnlightener messageHandler, string accessTokenOrApi)
        {
            IResponseMessageBase responseMessage = null;
            responseConfigs = responseConfigs ?? new List<Response>();
            if (responseConfigs.Count == 0)
            {
                return null;
            }

            //获取第一个响应设置
            var firstResponse = responseConfigs.First();
            //从第二个响应开始：扩展响应
            var extendReponses = responseConfigs.Count > 1
                    ? responseConfigs.Skip(1).Take(responseConfigs.Count - 1).ToList()
                    : new List<Response>();

            //处理特殊情况
            if (messageHandler.MessageEntityEnlightener.PlatformType == PlatformType.WeChat_MiniProgram)
            {
                //小程序，所有的请求都使用客服消息接口
                extendReponses.Insert(0, firstResponse);
                firstResponse = new Response() { Type = ResponseMsgType.SuccessResponse };//返回成功信息
                responseMessage = new SuccessResponseMessage();
            }


            //第一项，优先使用消息回复
            switch (firstResponse.Type)
            {
                case ResponseMsgType.Text:
                    responseMessage = RenderResponseMessageText(requestMessage, firstResponse, messageHandler.MessageEntityEnlightener);
                    break;
                case ResponseMsgType.News:
                    break;
                case ResponseMsgType.Music:
                    break;
                case ResponseMsgType.Image:
                    responseMessage = RenderResponseMessageImage(requestMessage, firstResponse, messageHandler.MessageEntityEnlightener);
                    break;
                case ResponseMsgType.Voice:
                    break;
                case ResponseMsgType.Video:
                    break;
                case ResponseMsgType.Transfer_Customer_Service:
                    break;
                case ResponseMsgType.MultipleNews:
                    break;
                case ResponseMsgType.LocationMessage:
                    break;
                case ResponseMsgType.NoResponse:
                    responseMessage = RenderResponseMessageNoResponse(requestMessage, firstResponse, messageHandler.MessageEntityEnlightener);
                    break;
                case ResponseMsgType.SuccessResponse:
                    responseMessage = RenderResponseMessageNoResponse(requestMessage, firstResponse, messageHandler.MessageEntityEnlightener);
                    break;
                default:
                    break;
            }

            //使用客服接口（高级接口）发送
            ExecuteApi(extendReponses, requestMessage, messageHandler.ApiEnlightener, accessTokenOrApi, requestMessage.FromUserName);

            return responseMessage;
        }

        /// <summary>
        /// 执行API高级接口回复
        /// </summary>
        /// <param name="responses"></param>
        /// <param name="requestMessage"></param>
        /// <param name="apiEnlighten"></param>
        /// <param name="accessTokenOrApi"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        private List<ApiResult> ExecuteApi(List<Response> responses, IRequestMessageBase requestMessage, ApiEnlightener apiEnlighten, string accessTokenOrApi, string openId)
        {
            List<ApiResult> results = new List<ApiResult>();

            if (responses == null || responses.Count == 0)
            {
                return results;
            }

            ApiHandler apiHandler = new ApiHandler(apiEnlighten);
            foreach (var response in responses)
            {
                ApiResult apiResult = apiHandler.ExecuteApi(response, requestMessage, accessTokenOrApi, openId);
                results.Add(apiResult);
            }
            return results;
        }

        /// <summary>
        /// 返回文字类型信息
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="responseConfig"></param>
        /// <returns></returns>
        private IResponseMessageText RenderResponseMessageText(IRequestMessageBase requestMessage, Response responseConfig, MessageEntityEnlightener enlighten)
        {
            var strongResponseMessage = requestMessage.CreateResponseMessage<IResponseMessageText>(enlighten);
            strongResponseMessage.Content = NeuralNodeHelper.FillTextMessage(responseConfig.Content);
            return strongResponseMessage;
        }

        /// <summary>
        /// 返回图片类型信息
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="responseConfig"></param>
        /// <returns></returns>
        private IResponseMessageBase RenderResponseMessageImage(IRequestMessageBase requestMessage, Response responseConfig, MessageEntityEnlightener enlighten)
        {
            var strongResponseMessage = requestMessage.CreateResponseMessage<IResponseMessageImage>(enlighten);
            var mediaId = NeuralNodeHelper.GetImageMessageMediaId(requestMessage, responseConfig.Content);
            if (string.IsNullOrEmpty(mediaId))
            {
                var textResponseMessage = requestMessage.CreateResponseMessage<IResponseMessageText>(enlighten);
                textResponseMessage.Content = "消息中未获取到图片信息";
                return textResponseMessage;
            }
            else
            {
                strongResponseMessage.Image.MediaId = mediaId;
            }


            //TODO：其他情况

            return strongResponseMessage;
        }

        /// <summary>
        /// 返回图片类型信息
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="responseConfig"></param>
        /// <returns></returns>
        private IResponseMessageBase RenderResponseMessageNoResponse(IRequestMessageBase requestMessage, Response responseConfig, MessageEntityEnlightener enlighten)
        {
            var strongResponseMessage = requestMessage.CreateResponseMessage<ResponseMessageNoResponse>(enlighten);
            var mediaId = NeuralNodeHelper.GetImageMessageMediaId(requestMessage, responseConfig.Content);
            return strongResponseMessage;
        }


        #endregion
    }

}
