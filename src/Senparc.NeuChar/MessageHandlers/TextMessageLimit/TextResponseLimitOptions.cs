using Senparc.NeuChar.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senparc.NeuChar.MessageHandlers
{
    /// <summary>
    /// 限制返回消息长度，如果超出，则调用客服接口进行发送
    /// </summary>
    public class TextResponseLimitOptions
    {
        /// <summary>
        /// 需要限制的长度
        /// </summary>
        public int MaxTextBytesLimit { get; set; }
        /// <summary>
        /// 已经注册完的 AppId 或 AccessToken
        /// </summary>
        public string AccessTokenOrAppId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxTextBytesLimit">需要限制的长度（字节），默认为 2048</param>
        /// <param name="accessTokenOrAppId"></param>
        public TextResponseLimitOptions(int maxTextBytesLimit = 2048, string accessTokenOrAppId = null)
        {
            MaxTextBytesLimit = maxTextBytesLimit;
            AccessTokenOrAppId = accessTokenOrAppId;
        }

        /// <summary>
        /// 设置 accessTokenOrAppId
        /// </summary>
        /// <param name="accessTokenOrAppId"></param>
        public void SetAccessTokenOrAppId(string accessTokenOrAppId)
        {
            AccessTokenOrAppId = accessTokenOrAppId;
        }

        /// <summary>
        /// 检查消息是否超出限制，如果超出，则使用客服消息接口，并且返回 false，否则返回 true
        /// </summary>
        /// <param name="responseMessageText">文字回复消息</param>
        /// <param name="messageHandler">MessageHandler</param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public async Task<bool> CheckOrSendCustomMessage( IResponseMessageText responseMessageText, IMessageHandlerEnlightener messageHandler, string openId)
        {
            var origStr = responseMessageText.Content;
            byte[] bytes = System.Text.Encoding.Unicode.GetBytes(origStr);
            if (bytes.Length > MaxTextBytesLimit)
            {
                //使用客服消息

                //TODO:判断 accessTokenOrAppId 是否为空，可以抛出异常

                await messageHandler.ApiEnlightener.SendText(AccessTokenOrAppId, openId, origStr, MaxTextBytesLimit);

                return false;
            }

            return true;
        }

        /// <summary>
        /// 根据字节长度来截取字符串
        /// </summary>
        ///<param name="origStr">原始字符串</param>
        ///<param name="length">提取前length个字节</param>
        /// <returns></returns>
        public String SubstringByByte(string origStr, int length)
        {
            byte[] bytes = System.Text.Encoding.Unicode.GetBytes(origStr);//TODO:此方法也有不被推荐的理由：https://www.cnblogs.com/wang-jin-fu/p/11795077.html
            int n = 0; //  表示当前的字节数
            int i = 0; //  要截取的字节数
            for (; i < bytes.GetLength(0) && n < length; i++)
            {
                //  偶数位置，如0、2、4等，为UCS2编码中两个字节的第一个字节
                if (i % 2 == 0)
                {
                    n++; //  在UCS2第一个字节时n加1
                }
                else
                {
                    //  当UCS2编码的第二个字节大于0时，该UCS2字符为汉字，一个汉字算两个字节
                    if (bytes[i] > 0)
                    {
                        n++;
                    }
                }
            }
            //  如果i为奇数时，处理成偶数
            if (i % 2 == 1)
            {
                //  该UCS2字符是汉字时，去掉这个截一半的汉字

                if (bytes[i] > 0)
                    i = i - 1;

                //  该UCS2字符是字母或数字，则保留该字符
                else
                    i = i + 1;
            }
            return Encoding.Unicode.GetString(bytes, 0, i);
        }
    }
}
