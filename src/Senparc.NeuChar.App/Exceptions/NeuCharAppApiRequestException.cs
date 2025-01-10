using Senparc.CO2NET.Exceptions;
using Senparc.NeuChar.App.AppStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senparc.NeuChar.App.Exceptions
{
    /// <summary>
    /// NeuCharApp 异常
    /// </summary>
    public class NeuCharAppApiRequestException : NeuCharAppException
    {
        /// <summary>
        /// JsonResult
        /// </summary>
        public ApiJsonResult JsonResult { get; set; }
        /// <summary>
        /// 接口 URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// ErrorJsonResultException
        /// </summary>
        /// <param name="message">异常消息</param>
        /// <param name="inner">内部异常</param>
        /// <param name="jsonResult">WxJsonResult</param>
        /// <param name="url">API地址</param>
        public NeuCharAppApiRequestException(string message, Exception inner, ApiJsonResult jsonResult, string url = null)
            : base(message, inner)
        {
            JsonResult = jsonResult;
            Url = url;
        }
    }
}