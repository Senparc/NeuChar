using Senparc.CO2NET.Helpers;
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
        /// 处理图文消息
        /// </summary>
        /// <param name="originContent"></param>
        /// <returns></returns>
        public static List<Article> FillNewsMessage(string originContent)
        {
            var list = SerializerHelper.GetObject<List<Article>>(originContent);

            foreach (var item in list)
            {
                item.Title = FillTextMessage(item.Title);
                item.Description = FillTextMessage(item.Description);
            }

            return list;
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
