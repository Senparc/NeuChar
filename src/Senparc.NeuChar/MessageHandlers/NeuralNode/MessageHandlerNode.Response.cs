using Senparc.NeuChar.ApiHandlers;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Helpers;
using Senparc.NeuChar.MessageHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.NeuralSystems
{
    public partial class MessageHandlerNode
    {
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

            //扩展响应，会在消息回复之前发送
            var extendReponses = responseConfigs.Count > 1
                    ? responseConfigs.Take(responseConfigs.Count - 1).ToList()
                    : new List<Response>();

            //获取最后一个响应设置
            var lastResponse = responseConfigs.Last();//取最后一个

            //处理特殊情况
            if (messageHandler.MessageEntityEnlightener.PlatformType == PlatformType.WeChat_MiniProgram)
            {
                //小程序，所有的请求都使用客服消息接口，回填取出的最后一个
                extendReponses.Add(lastResponse);
                lastResponse = new Response() { Type = ResponseMsgType.SuccessResponse };//返回成功信息
                                                                                         //responseMessage = new SuccessResponseMessage();
            }

            //排序的最后一项，使用 MesageHandler 的普通消息回复
            switch (lastResponse.Type)
            {
                case ResponseMsgType.Text:
                    responseMessage = RenderResponseMessageText(requestMessage, lastResponse, messageHandler.MessageEntityEnlightener);

                    break;
                case ResponseMsgType.News:
                case ResponseMsgType.MultipleNews:
                    responseMessage = RenderResponseMessageNews(requestMessage, lastResponse, messageHandler.MessageEntityEnlightener);
                    break;
                case ResponseMsgType.Music:
                    break;
                case ResponseMsgType.Image:
                    responseMessage = RenderResponseMessageImage(requestMessage, lastResponse, messageHandler.MessageEntityEnlightener);
                    break;
                case ResponseMsgType.Voice:
                    break;
                case ResponseMsgType.Video:
                    break;
                case ResponseMsgType.Transfer_Customer_Service:
                    break;
                //case ResponseMsgType.MultipleNews:
                //    break;
                case ResponseMsgType.LocationMessage:
                    break;
                case ResponseMsgType.NoResponse:
                    responseMessage = RenderResponseMessageNoResponse(requestMessage, lastResponse, messageHandler.MessageEntityEnlightener);
                    break;
                case ResponseMsgType.SuccessResponse:
                    responseMessage = RenderResponseMessageSuccessResponse(requestMessage, lastResponse, messageHandler.MessageEntityEnlightener);
                    break;
                case ResponseMsgType.UseApi://常规官方平台转发的请求不会到达这里
                    break;
                default:
                    break;
            }

            //使用客服接口（高级接口）发送最后一项之前的所有信息
            ExecuteApi(extendReponses, requestMessage, messageHandler, accessTokenOrApi, requestMessage.FromUserName);

            return responseMessage;
        }

        /// <summary>
        /// 执行API高级接口回复
        /// </summary>
        /// <param name="responses"></param>
        /// <param name="requestMessage"></param>
        /// <param name="enlightener"></param>
        /// <param name="accessTokenOrApi"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        private List<ApiResult> ExecuteApi(List<Response> responses, IRequestMessageBase requestMessage, IMessageHandlerEnlightener enlightener, string accessTokenOrApi, string openId)
        {
            List<ApiResult> results = new List<ApiResult>();

            if (responses == null || responses.Count == 0)
            {
                return results;
            }

            ApiHandler apiHandler = new ApiHandler(enlightener.ApiEnlightener);
            foreach (var response in responses)
            {
                ApiResult apiResult = apiHandler.ExecuteApi(response, MaterialData, requestMessage, accessTokenOrApi, openId, enlightener);
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
        private IResponseMessageBase RenderResponseMessageText(IRequestMessageBase requestMessage, Response responseConfig, MessageEntityEnlightener enlighten)
        {
            var strongResponseMessage = requestMessage.CreateResponseMessage<IResponseMessageText>(enlighten);
            strongResponseMessage.Content = NeuralNodeHelper.FillTextMessage(responseConfig.GetMaterialContent(MaterialData));
            return strongResponseMessage;
        }

        /// <summary>
        /// 返回图文类型信息
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="responseConfig"></param>
        /// <param name="enlighten"></param>
        /// <returns></returns>
        private IResponseMessageBase RenderResponseMessageNews(IRequestMessageBase requestMessage, Response responseConfig, MessageEntityEnlightener enlighten)
        {
            var articles = NeuralNodeHelper.FillNewsMessage(responseConfig.MaterialId, MaterialData);

            var strongResponseMessage = requestMessage.CreateResponseMessage<IResponseMessageNews>(enlighten);
            if (articles != null)
            {
                strongResponseMessage.Articles = articles;
            }
            else
            {
                strongResponseMessage.Articles = new List<Article>() {
                    new Article() {
                     Title="您要查找的素材不存在，或格式定义错误！",
                     Description="您要查找的素材不存在，或格式定义错误！"
                } };
            }
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
            var mediaId = NeuralNodeHelper.GetImageMessageMediaId(requestMessage, responseConfig.GetMaterialContent(MaterialData));
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
        /// 不返回任何响应消息
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="responseConfig"></param>
        /// <returns></returns>
        private IResponseMessageBase RenderResponseMessageNoResponse(IRequestMessageBase requestMessage, Response responseConfig, MessageEntityEnlightener enlighten)
        {
            var strongResponseMessage = requestMessage.CreateResponseMessage<ResponseMessageNoResponse>(enlighten);
            return strongResponseMessage;
        }

        /// <summary>
        /// 回复“成功”信息（默认为字符串success）
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="responseConfig"></param>
        /// <returns></returns>
        private IResponseMessageBase RenderResponseMessageSuccessResponse(IRequestMessageBase requestMessage, Response responseConfig, MessageEntityEnlightener enlighten)
        {
            var strongResponseMessage = requestMessage.CreateResponseMessage<SuccessResponseMessage>(enlighten);
            return strongResponseMessage;
        }


        #endregion

    }
}
