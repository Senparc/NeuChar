using Senparc.NeuChar.MessageHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.ApiHandlers
{
    /// <summary>
    /// 
    /// </summary>
    public class ApiHandler
    {
        public ApiEnlighten ApiEnlighten { get; set; }

        /// <summary>
        /// ApiHandler
        /// </summary>
        /// <param name="apiEnlighten"></param>
        public ApiHandler(ApiEnlighten apiEnlighten)
        {
            ApiEnlighten = apiEnlighten;
        }

        /// <summary>
        /// 执行API
        /// </summary>
        /// <param name="response"></param>
        /// <param name="accessTokenOrAppId"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public ApiResult ExecuteApi(Response response, string accessTokenOrAppId, string openId)
        {
            if (response == null)
            {
                return new ApiResult(-1, "未获取到响应消息设置", null);
            }

            ApiResult apiResult = null;

            switch (response.Type)
            {
                case ResponseMsgType.Unknown:
                    break;
                case ResponseMsgType.Text:
                    apiResult = ApiEnlighten.SendText(accessTokenOrAppId, openId, response.Content);
                    break;
                case ResponseMsgType.News:
                    break;
                case ResponseMsgType.Music:
                    break;
                case ResponseMsgType.Image:
                    apiResult = ApiEnlighten.SendImage(accessTokenOrAppId, openId, response.Content);
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
                default:
                    apiResult = new ApiResult(-1, "未找到支持的响应消息类型", null);
                    break;
            }

            return apiResult;
        }
    }
}
