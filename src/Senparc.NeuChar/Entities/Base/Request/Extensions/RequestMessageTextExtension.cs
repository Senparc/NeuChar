#region Apache License Version 2.0
/*----------------------------------------------------------------

Copyright 2018 Suzhou Senparc Network Technology Co.,Ltd.

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
    Copyright (C) 2018 Senparc
 
    文件名：TenPayV3.cs
    文件功能描述：微信支付V3接口
    
    
    创建标识：Senparc - 20170422
    
    修改标识：Senparc - 20170725
    修改描述：v4.13.2 添加RequestMessageTextExtension的大小写是否敏感设置

    修改标识：Senparc - 20170725
    修改描述：v4.14.2 修复RequestMessageTextExtension.GetResponseMessage()方法判断问题

    修改标识：Senparc - 20190309
    修改描述：v0.6.3 添加 SelectMenuKeyword() 扩展方法
     
    修改标识：Senparc - 20190329
    修改描述：v0.6.4 方法中添加对 SelectMenuId 是否为空的判断
     
----------------------------------------------------------------*/


using Senparc.CO2NET.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Senparc.NeuChar.Entities.Request
{
    public class RequestMessageTextKeywordHandler
    {
        /// <summary>
        /// 选择菜单（对应微信 SendMenu 接口）的关键字格式，{0}代表客户端识别的关键字
        /// </summary>
        public const string SELECT_MENU_KEWORD_FORMAT = "{0}";//这里保留这个方法，为以后扩充做准备

        internal string Keyword { get; set; }

        /// <summary>
        /// DefaultMessage
        /// </summary>
        internal Func<ResponseMessageBase> DefaultMessage;

        internal IRequestMessageText RequestMessage { get; set; }

        public IResponseMessageBase ResponseMessage { get; set; }

        /// <summary>
        /// 选择菜单Id（对应微信 SendMenu 接口所设置的 id 参数）
        /// </summary>
        public string SelectMenuId { get; set; }

        /// <summary>
        /// 是否已经匹配成功
        /// </summary>
        public bool MatchSuccessed { get; set; }

        /// <summary>
        /// 是否大小写敏感
        /// </summary>
        public bool CaseSensitive { get; set; }


        public RequestMessageTextKeywordHandler(IRequestMessageText requestMessage, bool caseSensitive = false)
        {
            RequestMessage = requestMessage;
            CaseSensitive = caseSensitive;
            Keyword = RequestMessage.Content;
            if (requestMessage is IRequestMessageSelectMenu selectMenuRequestMessage)
            {
                SelectMenuId = selectMenuRequestMessage.bizmsgmenuid;
            }
        }
    }

    /// <summary>
    /// RequestMessageText 扩展
    /// </summary>
    public static class RequestMessageTextExtension
    {
        /// <summary>
        /// 开始匹配
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="caseSensitive">是否大小写敏感，默认为false</param>
        /// <returns></returns>
        public static RequestMessageTextKeywordHandler StartHandler(this IRequestMessageText requestMessage, bool caseSensitive = false)
        {
            var handler = new RequestMessageTextKeywordHandler(requestMessage, caseSensitive);
            return handler;
        }

        /// <summary>
        /// 获取最终响应消息
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static IResponseMessageBase GetResponseMessage(this RequestMessageTextKeywordHandler handler)
        {
            if (!handler.MatchSuccessed
                && handler.DefaultMessage != null)
            {
                handler.ResponseMessage = handler.DefaultMessage();
            }
            return handler.ResponseMessage;
        }


        /// <summary>
        /// 匹配关键词
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="keyword">关键词</param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static RequestMessageTextKeywordHandler Keyword(this RequestMessageTextKeywordHandler handler, string keyword, Func<IResponseMessageBase> func)
        {
            if (!handler.MatchSuccessed &&
                ((handler.CaseSensitive && handler.Keyword == keyword) ||
                (!handler.CaseSensitive && handler.Keyword.ToUpper() == keyword.ToUpper())))
            {
                handler.MatchSuccessed = true;
                handler.ResponseMessage = func();
            }
            return handler;
        }

        /// <summary>
        /// 匹配关键词（只要有一个满足即可触发）
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="keywords">多个关键词</param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static RequestMessageTextKeywordHandler Keywords(this RequestMessageTextKeywordHandler handler, string[] keywords, Func<IResponseMessageBase> func)
        {
            foreach (var keyword in keywords)
            {
                handler.Keyword(keyword, func);
            }
            return handler;
        }


        /// <summary>
        /// 匹配选择菜单关键词（对应微信的 SendMenu 接口）
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="keywords">多个关键词（匹配格式："s:[关键词]"）</param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static RequestMessageTextKeywordHandler SelectMenuKeyword(this RequestMessageTextKeywordHandler handler, string keyword, Func<IResponseMessageBase> func)
        {
            if (!(handler.RequestMessage is IRequestMessageSelectMenu))
            {
                throw new Exceptions.MessageHandlerException($"当前请求类型 {handler.RequestMessage.GetType()} 未实现 {nameof(IRequestMessageSelectMenu)} 接口，因此无法使用此方法（{nameof(SelectMenuKeyword)}）。");
            }

            var finalKeyword = RequestMessageTextKeywordHandler.SELECT_MENU_KEWORD_FORMAT.FormatWith(keyword);
            if (!handler.MatchSuccessed && !handler.SelectMenuId.IsNullOrWhiteSpace() &&
               ((handler.CaseSensitive && handler.SelectMenuId == finalKeyword ||
               (!handler.CaseSensitive && handler.SelectMenuId.ToUpper() == finalKeyword.ToUpper()))))
                {
                handler.MatchSuccessed = true;
                handler.ResponseMessage = func();
            }
            return handler;
        }


        /// <summary>
        /// 匹配选择菜单关键词（对应微信的 SendMenu 接口）
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="keywords">多个关键词（匹配格式："s:[关键词]"）</param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static RequestMessageTextKeywordHandler SelectMenuKeywords(this RequestMessageTextKeywordHandler handler, string[] keywords, Func<IResponseMessageBase> func)
        {
            foreach (var keyword in keywords)
            {
                handler.SelectMenuKeyword(keyword, func);
            }
            return handler;
        }

        /// <summary>
        /// 匹配正则表达式
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="pattern">正则表达式</param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static RequestMessageTextKeywordHandler Regex(this RequestMessageTextKeywordHandler handler, string pattern, Func<IResponseMessageBase> func)
        {
            if (!handler.MatchSuccessed
               && System.Text.RegularExpressions.Regex.IsMatch(handler.Keyword, pattern,
                    handler.CaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase))
            {
                handler.MatchSuccessed = true;
                handler.ResponseMessage = func();
            }
            return handler;
        }

        /// <summary>
        /// 默认消息
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        public static RequestMessageTextKeywordHandler Default(this RequestMessageTextKeywordHandler handler, Func<IResponseMessageBase> func)
        {
            if (!handler.MatchSuccessed)
            {
                handler.ResponseMessage = func();
            }
            return handler;
        }
    }
}
