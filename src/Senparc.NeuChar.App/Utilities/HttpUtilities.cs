using Senparc.CO2NET.Extensions;
using Senparc.CO2NET.Helpers;
using Senparc.CO2NET.Helpers.Serializers;
using Senparc.CO2NET.Trace;
using Senparc.NeuChar.App.AppStore;
using Senparc.NeuChar.App.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senparc.NeuChar.App.Utilities
{
    /// <summary>
    /// HttpUtilities
    /// </summary>
    public class HttpUtilities
    {
        /// <summary>
        /// 设定条件，当API结果没有返回成功信息时抛出异常
        /// </summary>
        static Action<string, string> getFailAction = (apiUrl, returnText) =>
        {
            SenparcTrace.SendApiLog(apiUrl, returnText);//TODO:为  API 提供 NeuChar 层面的日志记录

            if (returnText.Contains("errcode"))
            {
                //可能发生错误
                ApiJsonResult errorResult = SerializerHelper.GetObject<ApiJsonResult>(returnText);
                if (errorResult.errcode != 0/*ReturnCode.请求成功*/)
                {
                    //发生错误
                    throw new NeuCharAppApiRequestException(
                         string.Format("微信 GET 请求发生错误！错误代码：{0}，说明：{1}",
                                         errorResult.errcode, errorResult.errmsg), null, errorResult, apiUrl);
                }
            }
        };

        /// <summary>
        /// 设定条件，当API结果没有返回成功信息时抛出异常
        /// </summary>
        static Action<string, string> postFailAction = (apiUrl, returnText) =>
        {
            if (returnText.Contains("errcode"))
            {
                //可能发生错误
                ApiJsonResult errorResult = SerializerHelper.GetObject<ApiJsonResult>(returnText);
                if (errorResult.errcode != 0/*ReturnCode.请求成功*/)
                {
                    //发生错误
                    throw new NeuCharAppApiRequestException(
                         string.Format("微信 POST 请求发生错误！错误代码：{0}，说明：{1}",
                                       errorResult.errcode, errorResult.errmsg),
                         null, errorResult, apiUrl);
                }
            }
        };

        #region 同步方法
        /// <summary>
        /// 向需要AccessToken的API发送消息的公共方法
        /// </summary>
        /// <param name="accessToken">这里的AccessToken是通用接口的AccessToken，非OAuth的。如果不需要，可以为null，此时urlFormat不要提供{0}参数</param>
        /// <param name="urlFormat"></param>
        /// <param name="data">如果是Get方式，可以为null。在POST方式中将被转为JSON字符串提交</param>
        /// <param name="sendType">发送类型，POST或GET，默认为POST</param>
        /// <param name="timeOut">代理请求超时时间（毫秒）</param>
        /// <param name="checkValidationResult">验证服务器证书回调自动验证</param>
        /// <param name="jsonSetting">JSON字符串生成设置</param>
        /// <returns></returns>
        public static async Task<T> SendAsync<T>(string accessToken, string urlFormat, object data,
            HttpRequestType sendType = HttpRequestType.POST, int timeOut = CO2NET.Config.TIME_OUT,
            bool checkValidationResult = false, JsonSetting jsonSetting = null)
        {
            try
            {
                var url = string.IsNullOrEmpty(accessToken) ? urlFormat : string.Format(urlFormat, accessToken.AsUrlData());

                switch (sendType)
                {
                    case HttpRequestType.GET:
                        return await CO2NET.HttpUtility.Get.GetJsonAsync<T>(url, afterReturnText: getFailAction).ConfigureAwait(false);
                    case HttpRequestType.POST:
                        var jsonString = SerializerHelper.GetJsonString(data, jsonSetting);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            var bytes = Encoding.UTF8.GetBytes(jsonString);
                            await ms.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                            ms.Seek(0, SeekOrigin.Begin);

                            //WeixinTrace.SendApiPostDataLog(url, jsonString);//记录Post的Json数据
                            SenparcTrace.SendApiLog(url, jsonString);//记录Post的Json数据   TODO:为  API 提供 NeuChar 层面的日志记录

                            //PostGetJson方法中将使用WeixinTrace记录结果
                            return await CO2NET.HttpUtility.Post.PostGetJsonAsync<T>(url, null, ms,
                                timeOut: timeOut,
                                afterReturnText: postFailAction,
                                checkValidationResult: checkValidationResult).ConfigureAwait(false);
                        }
                    default:
                        throw new ArgumentOutOfRangeException("sendType");
                }
            }
            catch (NeuCharAppApiRequestException ex)
            {
                ex.Url = urlFormat;
                throw;
            }
        }

        #endregion
    }
}
