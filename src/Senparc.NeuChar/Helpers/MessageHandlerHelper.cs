﻿#region Apache License Version 2.0
/*----------------------------------------------------------------

Copyright 2023 Suzhou Senparc Network Technology Co.,Ltd.

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
    Copyright (C) 2023 Senparc
    
    文件名：MessageHandlerHelper.cs
    文件功能描述：MessageHandler 帮助类
    
    
    创建标识：Senparc - 20230709

----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Senparc.NeuChar.Helpers
{
    /// <summary>
    /// MessageHandler 帮助类
    /// </summary>
    public static class MessageHandlerHelper
    {
        ////来源：https://www.qqxiuzi.cn/zh/hanzi-unicode-bianma.php
        //private static List<int[]> chineseUnicodeArrange = new List<int[]>() {
        //    //汉字
        //    new[]{0x4E00, 0x9FA5   } ,
        //    new[]{0x9FA6, 0x9FFF   } ,
        //    new[]{0x3400, 0x4DBF   } ,
        //    new[]{0x20000, 0x2A6DF } ,
        //    new[]{0x2A700, 0x2B739 } ,
        //    new[]{0x2B740, 0x2B81D } ,
        //    new[]{0x2B820, 0x2CEA1 } ,
        //    new[]{0x2CEB0, 0x2EBE0 } ,
        //    new[]{0x30000, 0x3134A } ,
        //    new[]{0x31350, 0x323AF } ,
        //    new[]{0x2F00, 0x2FD5   } ,
        //    new[]{0x2E80, 0x2EF3   } ,
        //    new[]{0xF900, 0xFAD9   } ,
        //    new[]{0x2F800, 0x2FA1D } ,
        //    new[]{0x31C0, 0x31E3   } ,
        //    new[]{0x2FF0, 0x2FFB   } ,
        //    new[]{0x3105, 0x312F   } ,
        //    new[]{0x31A0, 0x31BF   } ,
        //    new[]{0x3007, 0x3007   }
        //};

        //private static bool SearchUnicode(char eachChar)
        //{
        //    for (int i = 0; i < chineseUnicodeArrange.Count; i++)
        //    {
        //        if (eachChar >= chineseUnicodeArrange[i][0] && eachChar <= chineseUnicodeArrange[i][1])
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        private static bool SearchUnicode(char eachChar)
        {
            return (int)eachChar > 127;
        }

        /// <summary>
        /// 计算字节数
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns></returns>
        public static int GetBytes(string str)
        {
            int charNum = 0; //统计字节位数
            char[] charArray = str.ToCharArray();
            for (int i = 0; i < charArray.Length; i++)
            {
                if (SearchUnicode(charArray[i])) //判断中文字符
                {
                    charNum += 3;//经过测试，微信计算 1 个中文等于 3 个字符，而不是 2 个
                }
                else
                {
                    charNum += 1;
                }
            }
            return charNum;
        }

        /// <summary>
        /// 取指定 byte 长度的字符串，如果遇到切割中文的情况，则最后一个中文不包括在结果内
        /// </summary>
        /// <param name="origStr">原始字符串</param>
        /// <param name="bytesLength">字符长度</param>
        /// <returns></returns>
        public static string SubstringByByte(string origStr, int bytesLength)
        {
            StringBuilder sb = new StringBuilder();
            char[] charArray = origStr.ToCharArray();
            var checkLength = 0;
            var i = 0;

            for (; i < charArray.Length; i++)
            {
                if (SearchUnicode(charArray[i])) //判断中文字符
                {
                    checkLength += 3;//经过测试，微信计算 1 个中文等于 3 个字符，而不是 2 个
                }
                else
                {
                    checkLength += 1;
                }

                if (checkLength > bytesLength)
                {
                    break;
                }

                sb.Append(charArray[i]);
            }

            return sb.ToString();
        }

        ///// <summary>
        ///// 根据字节长度来截取字符串
        ///// </summary>
        /////<param name="origStr">原始字符串</param>
        /////<param name="length">提取前length个字节</param>
        ///// <returns></returns>
        //public static string SubstringByByte(string origStr, int length)
        //{
        //    byte[] bytes = System.Text.Encoding.Unicode.GetBytes(origStr);//TODO:此方法也有不被推荐的理由：https://www.cnblogs.com/wang-jin-fu/p/11795077.html
        //    int n = 0; //  表示当前的字节数
        //    int i = 0; //  要截取的字节数
        //    for (; i < bytes.GetLength(0) && n < length; i++)
        //    {
        //        //  偶数位置，如0、2、4等，为UCS2编码中两个字节的第一个字节
        //        if (i % 2 == 0)
        //        {
        //            n++; //  在UCS2第一个字节时n加1
        //        }
        //        else
        //        {
        //            //  当UCS2编码的第二个字节大于0时，该UCS2字符为汉字，一个汉字算两个字节
        //            if (bytes[i] > 0)
        //            {
        //                n++;
        //            }
        //        }
        //    }
        //    //  如果i为奇数时，处理成偶数
        //    if (i % 2 == 1)
        //    {
        //        //  该UCS2字符是汉字时，去掉这个截一半的汉字

        //        if (bytes[i] > 0)
        //            i = i - 1;

        //        //  该UCS2字符是字母或数字，则保留该字符
        //        else
        //            i = i + 1;
        //    }
        //    return Encoding.Unicode.GetString(bytes, 0, i);
        //}

        /// <summary>
        /// 尝试发送超长的内容，如果没有超长，则返回 null
        /// </summary>
        /// <param name="accessTokenOrAppId"></param>
        /// <param name="content"></param>
        /// <param name="limitedBytes"></param>
        /// <param name="sendTextFuncAsync"></param>
        /// <returns></returns>
        public static async Task<T> TrySendLimistedText<T>(string accessTokenOrAppId, string content, int limitedBytes, Func<string, Task<T>> sendTextFuncAsync)
            where T : class
        {
            if (limitedBytes > 0)
            {
                var bytesLength = MessageHandlerHelper.GetBytes(content);
                if (bytesLength > limitedBytes)
                {
                    var currentSendText = MessageHandlerHelper.SubstringByByte(content, limitedBytes);

                    //发送第一批
                    var result = await sendTextFuncAsync(currentSendText);

                    var lastText = content.Substring(currentSendText.Length, content.Length - currentSendText.Length);

                    //发送剩下的
                    return await sendTextFuncAsync(lastText);
                }
                else
                {
                    return null;
                }
            }

            return null;//不做处理
        }

        /// <summary>
        /// 尝试使用Unicode编码分批处理超长的文本内容，返回处理结果集合
        /// </summary>
        /// <param name="content">文本内容</param>
        /// <param name="limitedBytes">每段文本的限制长度</param>
        /// <param name="handleTextFuncAsync">处理方法</param>
        /// <returns>处理结果集合</returns>
        public static async Task<IEnumerable<T>> TryHandleLimitedText<T>(string content, int limitedBytes, Func<string, Task<T>> handleTextFuncAsync)
            where T : class
        {
            List<T> results = new();

            if (limitedBytes > 0)
            {
                foreach (var chunk in ChunkStringByUnicode(content, limitedBytes))
                {
                    results.Add(await handleTextFuncAsync(chunk));
                }
            }

            return results;
        }

        /// <summary>
        /// 使用Unicode编码对文本进行拆分
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <param name="chunkSize">分片大小</param>
        /// <returns></returns>
        public static IEnumerable<string> ChunkStringByUnicode(string text, int chunkSize)
        {
            var stringBuilder = new StringBuilder();
            var byteSize = 0;
            TextElementEnumerator enumerator = StringInfo.GetTextElementEnumerator(text);

            while (enumerator.MoveNext())
            {
                string unicodeCharacter = enumerator.GetTextElement();
                var b = Encoding.UTF8.GetBytes(unicodeCharacter);
                if (byteSize + b.Length >= chunkSize)
                {
                    yield return stringBuilder.ToString();
                    stringBuilder.Clear();
                    byteSize = 0;
                }
                byteSize += b.Length;
                stringBuilder.Append(unicodeCharacter);
            }
            yield return stringBuilder.ToString();
        }
    }
}
