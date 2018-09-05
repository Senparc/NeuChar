using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.ApiHandlers
{
    /// <summary>
    /// API信息初始化
    /// </summary>
    public abstract class ApiEnlighten
    {
        /// <summary>
        /// 发送文本消息
        /// </summary>
        /// <param name="accessTokenOrApi"></param>
        /// <param name="openId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public abstract ApiResult SendText(string accessTokenOrAppId, string openId, string content);

        /// <summary>
        /// 发送图片消息
        /// </summary>
        /// <param name="accessTokenOrApi"></param>
        /// <param name="openId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public abstract ApiResult SendImage(string accessTokenOrAppId, string openId, string mediaId);

    }
}
