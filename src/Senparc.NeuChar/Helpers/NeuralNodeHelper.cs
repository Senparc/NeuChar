using Senparc.NeuChar.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.Helpers
{
    public class NeuralNodeHelper
    {

        /// <summary>
        /// 处理文本信息占位符
        /// </summary>
        /// <param name="originContent"></param>
        /// <returns></returns>
        public static string FillTextMessage(string originContent)
        {
            return originContent.Replace("{now}", DateTime.Now.ToString());
        }

        /// <summary>
        /// 处理图片信息MediaId填充
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="originContent"></param>
        /// <returns></returns>
        public static string GetImageMessageMediaId(IRequestMessageBase requestMessage, string originContent)
        {
            if (originContent!=null && originContent.Equals("{current_img}", StringComparison.OrdinalIgnoreCase) && 
                requestMessage is IRequestMessageImage)
            {
                return (requestMessage as IRequestMessageImage).MediaId;
            }

            return originContent;
        }
    }
}
