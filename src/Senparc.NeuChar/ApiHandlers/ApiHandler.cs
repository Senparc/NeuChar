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
    
    文件名：ApiHandler.cs
    文件功能描述：ApiHandler
    
    
    创建标识：Senparc - 20180905

    修改标识：Senparc - 20191004
    修改描述：改为以异步方法为主
----------------------------------------------------------------*/

using Senparc.CO2NET.Trace;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Exceptions;
using Senparc.NeuChar.Helpers;
using Senparc.NeuChar.MessageHandlers;
using Senparc.NeuChar.NeuralSystems;
using System;
using System.Threading.Tasks;

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
        public async Task<ApiResult> ExecuteApi(Response response, MaterialData materialData, IRequestMessageBase requestMessage, string accessTokenOrAppId, string openId, IMessageHandlerEnlightener enlightener)
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
                        apiResult = await ApiEnlighten.SendText(accessTokenOrAppId, openId, cotnent);
                        break;
                    case ResponseMsgType.News:
                        {
                            var articles = NeuralNodeHelper.FillNewsMessage(response.MaterialId/*"9DAAC45C|6309EAD9"*/, materialData);
                            if (articles == null)
                            {
                                apiResult = await ApiEnlighten.SendText(accessTokenOrAppId, openId, "您要查找的素材不存在，或格式定义错误！");
                            }
                            else
                            {
                                apiResult = await ApiEnlighten.SendNews(accessTokenOrAppId, openId, articles);
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
                        apiResult = await ApiEnlighten.SendImage(accessTokenOrAppId, openId, mediaId);
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
                        apiResult = await ApiEnlighten.CustomApi(response, materialData, requestMessage.FromUserName);
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
