using Senparc.CO2NET.Helpers;
using Senparc.CO2NET.Trace;
using Senparc.NeuChar.ApiBind;
using Senparc.NeuChar.Enlightener;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Exceptions;
using Senparc.NeuChar.Helpers;
using Senparc.NeuChar.NeuralSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.ApiHandlers
{
    /// <summary>
    /// API信息初始化
    /// </summary>
    public abstract class ApiEnlightener : IEnlightener
    {
        /// <summary>
        /// 支持平台类型
        /// </summary>
        public abstract PlatformType PlatformType { get; set; }

        /// <summary>
        /// 发送文本消息
        /// </summary>
        /// <param name="accessTokenOrAppId"></param>
        /// <param name="openId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public abstract ApiResult SendText(string accessTokenOrAppId, string openId, string content);
        /// <summary>
        /// 发送多图文信息
        /// </summary>
        /// <param name="accessTokenOrAppId"></param>
        /// <param name="openId"></param>
        /// <param name="articleList"></param>
        /// <returns></returns>

        public abstract ApiResult SendNews(string accessTokenOrAppId, string openId, List<Article> articleList);

        /// <summary>
        /// 发送图片消息
        /// </summary>
        /// <param name="accessTokenOrAppId"></param>
        /// <param name="openId"></param>
        /// <param name="mediaId"></param>
        /// <returns></returns>
        public abstract ApiResult SendImage(string accessTokenOrAppId, string openId, string mediaId);


        /// <summary>
        /// 调用自定义接口（使用ApiBind特性）
        /// </summary>
        /// <param name="response">响应设置信息</param>
        /// <param name="materialData"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public virtual ApiResult CustomApi(Response response, MaterialData materialData, string openId)
        {
            if (openId == null)
            {
                throw new ArgumentNullException(nameof(openId));
            }

            ApiBindJson apiBindJson = SerializerHelper.GetObject<ApiBindJson>(response.GetMaterialContent(materialData));
            if (apiBindJson == null)
            {
                throw new NeuCharException("自定义API设置格式错误或未提供设置！");
            }

            //查找映射
            ApiBindInfo apiBindInfo = ApiBindInfoCollection.Instance.Get(PlatformType, apiBindJson.name);
            if (apiBindInfo == null)
            {
                throw new NeuCharException($"自定义API未找到，名称：{apiBindJson.name}");
            }

            //开始使用反射调用
            try
            {
                var filledParameters = new List<object>();
                //foreach (var para in apiBindJson.parameters)
                //{
                //    if (para == null || !(para is string))
                //    {
                //        filledParameters.Add(para);
                //    }

                //    filledParameters.Add((para as string).Replace("{openid}", openId));
                //}


                var prarmeters = apiBindInfo.MethodInfo.GetParameters();
                for (int i = 0; i < prarmeters.Length; i++)
                {
                    if (apiBindJson.parameters != null && i < apiBindJson.parameters.Length)
                    {
                        //设置参数数量在方法参数数量范围以内
                        var para = apiBindJson.parameters[i];

                        if (para != null && para is string)
                        {

                            filledParameters.Add((para as string).Replace("{openid}", openId));
                        }
                        else
                        {
                            filledParameters.Add(para);
                        }
                    }
                    else
                    {
                        //设置参数数量在方法参数数量范围以外
                        filledParameters.Add(prarmeters[i].DefaultValue);
                    }
                }

                //TODO：判断是否为静态类，否则需要有实例
                var invokeResult = apiBindInfo.MethodInfo.Invoke(null, filledParameters.ToArray());

                var apiResult = new ApiResult(0, "success", invokeResult);
                return apiResult;
            }
            catch (Exception ex)
            {
                new NeuCharException(ex.Message, ex);//不抛出
                return new ApiResult(-1, ex.Message, null);
            }
        }
    }
}
