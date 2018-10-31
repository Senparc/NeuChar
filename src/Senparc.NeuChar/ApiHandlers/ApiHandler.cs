using Senparc.CO2NET.Trace;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Exceptions;
using Senparc.NeuChar.Helpers;
using Senparc.NeuChar.MessageHandlers;
using Senparc.NeuChar.NeuralSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.ApiHandlers
{
    /// <summary>
    /// ApiHandler
    /// </summary>
    public class ApiHandler
    {
        /// <summary>
        /// ApiEnlightener
        /// </summary>
        public ApiEnlightener ApiEnlighten { get; set; }

        /// <summary>
        /// ApiHandler
        /// </summary>
        /// <param name="apiEnlighten"></param>
        public ApiHandler(ApiEnlightener apiEnlighten)
        {
            ApiEnlighten = apiEnlighten;
        }

        /// <summary>
        /// 执行API
        /// </summary>
        /// <param name="response"></param>
        /// <param name="materialData"></param>
        /// <param name="requestMessage"></param>
        /// <param name="accessTokenOrAppId"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public ApiResult ExecuteApi(Response response, MaterialData materialData, IRequestMessageBase requestMessage, string accessTokenOrAppId, string openId, IMessageHandlerEnlightener enlightener)
        {
            if (response == null)
            {
                return new ApiResult(-1, "未获取到响应消息设置", null);
            }

            ApiResult apiResult = null;
            try
            {
                switch (response.Type)
                {
                    case ResponseMsgType.Unknown:
                        break;
                    case ResponseMsgType.Text:
                        var cotnent = NeuralNodeHelper.FillTextMessage(response.GetMaterialContent(materialData));
                        apiResult = ApiEnlighten.SendText(accessTokenOrAppId, openId, cotnent);
                        break;
                    case ResponseMsgType.News:
                        {
                            var articles = NeuralNodeHelper.FillNewsMessage(response.MaterialId/*"9DAAC45C|6309EAD9"*/, materialData);
                            if (articles == null)
                            {
                                apiResult = ApiEnlighten.SendText(accessTokenOrAppId, openId, "您要查找的素材不存在，或格式定义错误！");
                            }
                            else
                            {
                                apiResult = ApiEnlighten.SendNews(accessTokenOrAppId, openId, articles);
                            }
                        }
                        break;
                    case ResponseMsgType.Music:
                        break;
                    case ResponseMsgType.Image:
                        var mediaId = NeuralNodeHelper.GetImageMessageMediaId(requestMessage, response.GetMaterialContent(materialData));
                        SenparcTrace.SendCustomLog("ExecuteApi-Image", $"mediaId:{mediaId}");
                        if (true)
                        {
                            //TODO:其他mediaId的情况
                        }
                        apiResult = ApiEnlighten.SendImage(accessTokenOrAppId, openId, mediaId);
                        break;
                    case ResponseMsgType.Voice:
                        break;
                    case ResponseMsgType.Video:
                        break;
                    case ResponseMsgType.Transfer_Customer_Service:
                        break;
                    case ResponseMsgType.MpNews:
                        break;
                    case ResponseMsgType.MultipleNews:
                        break;
                    case ResponseMsgType.LocationMessage:
                        break;
                    case ResponseMsgType.NoResponse:
                        break;
                    case ResponseMsgType.SuccessResponse:
                        break;
                    case ResponseMsgType.UseApi:
                        apiResult = ApiEnlighten.CustomApi(response, materialData, requestMessage.FromUserName);
                        break;
                    default:
                        apiResult = new ApiResult(-1, "未找到支持的响应消息类型", null);
                        break;
                }
            }
            catch (Exception ex)
            {
                new MessageHandlerException("NeuChar API调用过程发生错误:" + ex.Message, ex);
                return new ApiResult(-1, "API调用过程发生错误！", null);
            }


            return apiResult;
        }
    }
}
