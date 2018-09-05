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
        /// <returns></returns>
        public IResponseMessageBase Execute(IRequestMessageBase requestMessage, ApiEnlighten apiEnlighten,string accessTokenOrApi)
        {
            IResponseMessageBase responseMessage = null;

            switch (requestMessage.MsgType)
            {
                case RequestMsgType.Text:
                    {
                        var textRequestMessage = requestMessage as IRequestMessageText;
                        //遍历所有的消息设置
                        foreach (var messagePair in Config.MessagePair.Where(z => z.Request.Type == RequestMsgType.Text))
                        {
                            //遍历每一个消息设置中的关键词
                            foreach (var keyword in messagePair.Request.Keywords)
                            {
                                if (keyword.Equals(textRequestMessage.Content, StringComparison.OrdinalIgnoreCase))//TODO:加入大小写敏感设计
                                {
                                    responseMessage = GetResponseMessage(requestMessage, messagePair.Response);
                                    ExecuteApi(messagePair, apiEnlighten, accessTokenOrApi, requestMessage.ToUserName);
                                    break;
                                }
                            }
                            if (responseMessage != null)
                            {
                                break;
                            }
                        }
                    }
                    break;
                case RequestMsgType.Image:
                    {
                        var imageRequestMessage = requestMessage as IRequestMessageImage;
                        //遍历所有的消息设置

                        foreach (var messagePair in Config.MessagePair.Where(z => z.Request.Type == RequestMsgType.Image))
                        {
                            responseMessage = GetResponseMessage(requestMessage, messagePair.Response);
                            ExecuteApi(messagePair, apiEnlighten, accessTokenOrApi, requestMessage.ToUserName);

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
            return responseMessage;
        }

#if !NET35 && !NET40
        /// <summary>
        /// 执行NeuChar判断过程，获取响应消息
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public async Task<IResponseMessageBase> ExecuteAsync(IRequestMessageBase requestMessage, ApiEnlighten apiEnlighten, string accessTokenOrApi)
        {
            return await Task.Run(() => Execute(requestMessage, apiEnlighten, accessTokenOrApi));
        }
#endif
        #region 返回信息

        private IResponseMessageBase GetResponseMessage(IRequestMessageBase requestMessage, Response responseConfig)
        {
            IResponseMessageBase responseMessage = null;
            switch (responseConfig.Type)
            {
                case ResponseMsgType.Text:
                    responseMessage = RenderResponseMessageText(requestMessage, responseConfig);
                    break;
                case ResponseMsgType.News:
                    break;
                case ResponseMsgType.Music:
                    break;
                case ResponseMsgType.Image:
                    responseMessage = RenderResponseMessageImage(requestMessage, responseConfig);
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
                    break;
                case ResponseMsgType.SuccessResponse:
                    break;
                default:
                    break;
            }
            return responseMessage;
        }

        private List<ApiResult> ExecuteApi(MessagePair messagePair,ApiEnlighten apiEnlighten,string accessTokenOrApi,string openId)
        {
            if (messagePair==null || messagePair.ExtendResponses.Count ==0)
            {
                return null;
            }
            ApiHandler apiHandler = new ApiHandler(apiEnlighten);
            List<ApiResult> results = new List<ApiResult>();
            foreach (var response in messagePair.ExtendResponses)
            {
                ApiResult apiResult = apiHandler.ExecuteApi(response, accessTokenOrApi, openId);
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
        private IResponseMessageText RenderResponseMessageText(IRequestMessageBase requestMessage, Response responseConfig)
        {
            var strongResponseMessage = requestMessage.CreateResponseMessage<IResponseMessageText>();
            strongResponseMessage.Content = responseConfig.Content.Replace("{now}", DateTime.Now.ToString());
            return strongResponseMessage;
        }

        /// <summary>
        /// 返回图片类型信息
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="responseConfig"></param>
        /// <returns></returns>
        private IResponseMessageBase RenderResponseMessageImage(IRequestMessageBase requestMessage, Response responseConfig)
        {
            var strongResponseMessage = requestMessage.CreateResponseMessage<IResponseMessageImage>();

            if (responseConfig.Content.Equals("{current_img}", StringComparison.OrdinalIgnoreCase))
            {
                var strongRequestMessage = requestMessage as IRequestMessageImage;
                if (strongRequestMessage != null)
                {
                    strongResponseMessage.Image.MediaId = strongRequestMessage.MediaId;
                }
                else
                {
                    var textResponseMessage = requestMessage.CreateResponseMessage<IResponseMessageText>();
                    textResponseMessage.Content = "消息中未获取到图片信息";
                    return textResponseMessage;
                }
            }
            else
            {
                //TODO：其他情况
            }

            return strongResponseMessage;
        }

        #endregion
    }

}
