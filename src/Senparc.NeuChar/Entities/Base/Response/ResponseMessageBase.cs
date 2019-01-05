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
    
    文件名：ResponseMessageBase.cs
    文件功能描述：响应回复消息基类
    
    
    创建标识：Senparc - 20150211
    
    修改标识：Senparc - 20150303
    修改描述：整理接口

    修改标识：Senparc - 20190902
    修改描述：添加 MsgType 接口
----------------------------------------------------------------*/


using Senparc.CO2NET.Exceptions;
using Senparc.NeuChar.Exceptions;
using Senparc.NeuChar.MessageHandlers;
using System;
using System.Xml.Linq;
using Senparc.NeuChar.Helpers;
using Senparc.CO2NET.Trace;

namespace Senparc.NeuChar.Entities
{
    /// <summary>
    /// 响应回复消息基类接口
    /// </summary>
	public interface IResponseMessageBase : IMessageBase
    {
        ResponseMsgType MsgType { get; set; }
        //string Content { get; set; }
        //bool FuncFlag { get; set; }


    }

    /// <summary>
    /// 响应回复消息基类
    /// </summary>
    public class ResponseMessageBase : MessageBase, IResponseMessageBase
    {
        public virtual ResponseMsgType MsgType { get; set; }

        public ResponseMessageBase() { }

        //public virtual ResponseMessageType MsgType
        //{
        //    get { return ResponseMessageType.Text; }
        //}

        /// <summary>
        /// 获取响应类型实例，并初始化
        /// <para>如果是直接调用，建议使用CreateFromRequestMessage&lt;T&gt;(IRequestMessageBase requestMessage)取代此方法</para>
        /// </summary>
        /// <param name="requestMessage">请求</param>
        /// <param name="msgType">响应类型</param>
        /// <returns></returns>
        //[Obsolete("建议使用CreateFromRequestMessage<T>(IRequestMessageBase requestMessage)取代此方法")]
        private static IResponseMessageBase CreateFromRequestMessage(IRequestMessageBase requestMessage, ResponseMsgType msgType, MessageEntityEnlightener enlighten)
        {
            IResponseMessageBase responseMessage = null;
            try
            {
                switch (msgType)
                {
                    case ResponseMsgType.Text:
                        responseMessage = enlighten.NewResponseMessageText();// new ResponseMessageText();
                        break;
                    case ResponseMsgType.News:
                        responseMessage = enlighten.NewResponseMessageNews();// new ResponseMessageNews();
                        break;
                    case ResponseMsgType.Music:
                        responseMessage = enlighten.NewResponseMessageMusic();// new ResponseMessageMusic();
                        break;
                    case ResponseMsgType.Image:
                        responseMessage = enlighten.NewResponseMessageImage();// new ResponseMessageImage();
                        break;
                    case ResponseMsgType.Voice:
                        responseMessage = enlighten.NewResponseMessageVoice();// new ResponseMessageVoice();
                        break;
                    case ResponseMsgType.Video:
                        responseMessage = enlighten.NewResponseMessageVideo();// new ResponseMessageVideo();
                        break;
                    case ResponseMsgType.Transfer_Customer_Service:
                        responseMessage = enlighten.NewResponseMessageTransfer_Customer_Service();// new ResponseMessageTransfer_Customer_Service();
                        break;
                    case ResponseMsgType.NoResponse:
                        responseMessage = new ResponseMessageNoResponse();
                        break;
                    default:
                        throw new UnknownRequestMsgTypeException(string.Format("ResponseMsgType没有为 {0} 提供对应处理程序。", msgType), new ArgumentOutOfRangeException());
                }
            }
            catch (Exception ex)
            {
                throw new MessageHandlerException("CreateFromRequestMessage过程发生异常", ex);
            }

            return responseMessage;
        }

        /// <summary>
        /// 获取响应类型实例，并初始化
        /// </summary>
        /// <typeparam name="T">需要返回的类型</typeparam>
        /// <param name="requestMessage">请求数据</param>
        /// <param name="enlighten">MessageEntityEnlighten，当 T 为接口时必须提供</param>
        /// <returns></returns>
        public static T CreateFromRequestMessage<T>(IRequestMessageBase requestMessage, MessageEntityEnlightener enlighten = null)
            where T : IResponseMessageBase
        {
            try
            {
                T responseMessage = default(T);

                var tType = typeof(T);

                if (tType.IsInterface)
                {
                    //是接口，需要使用 Enlightener
                    if (enlighten == null)
                    {
                        throw new MessageHandlerException("MessageEntityEnlighten 不能为 null");
                    }

                    var responseName = tType.Name.Replace("IResponseMessage", "").Replace("ResponseMessage", ""); //请求名称

                    ResponseMsgType msgType = (ResponseMsgType)Enum.Parse(typeof(ResponseMsgType), responseName);

                    responseMessage = (T)CreateFromRequestMessage(requestMessage, msgType, enlighten);
                }
                else
                {
                    //非接口，直接初始化
                    //Senparc.CO2NET.Helpers.ReflectionHelper.
                    responseMessage = (T)Activator.CreateInstance(tType);
                }

                if (responseMessage != null)
                {
                    responseMessage.ToUserName = requestMessage.FromUserName;
                    responseMessage.FromUserName = requestMessage.ToUserName;
                    responseMessage.CreateTime = SystemTime.Now; //使用当前最新时间
                }

                return responseMessage;
            }
            catch (Exception ex)
            {
                SenparcTrace.SendCustomLog("CreateFromRequestMessage异常调试", typeof(T).FullName);

                throw new BaseException("ResponseMessageBase.CreateFromRequestMessage<T>过程发生异常！", ex);
            }
        }

        /// <summary>
        /// 从返回结果XML转换成IResponseMessageBase实体类
        /// </summary>
        /// <param name="xml">返回给服务器的Response Xml</param>
        /// <param name="enlighten"></param>
        /// <returns></returns>
        public static IResponseMessageBase CreateFromResponseXml(string xml, MessageEntityEnlightener enlighten)
        {
            try
            {
                if (string.IsNullOrEmpty(xml))
                {
                    return null;
                }

                var doc = XDocument.Parse(xml);
                IResponseMessageBase responseMessage = null;
                var msgType = (ResponseMsgType)Enum.Parse(typeof(ResponseMsgType), doc.Root.Element("MsgType").Value, true);
                switch (msgType)
                {
                    case ResponseMsgType.Text:
                        responseMessage = enlighten.NewResponseMessageText();// new ResponseMessageText();
                        break;
                    case ResponseMsgType.News:
                        responseMessage = enlighten.NewResponseMessageNews();// new ResponseMessageNews();
                        break;
                    case ResponseMsgType.Music:
                        responseMessage = enlighten.NewResponseMessageMusic();// new ResponseMessageMusic();
                        break;
                    case ResponseMsgType.Image:
                        responseMessage = enlighten.NewResponseMessageImage();// new ResponseMessageImage();
                        break;
                    case ResponseMsgType.Voice:
                        responseMessage = enlighten.NewResponseMessageVoice();// new ResponseMessageVoice();
                        break;
                    case ResponseMsgType.Video:
                        responseMessage = enlighten.NewResponseMessageVideo();// new ResponseMessageVideo();
                        break;
                    case ResponseMsgType.Transfer_Customer_Service:
                        responseMessage = enlighten.NewResponseMessageTransfer_Customer_Service();// new ResponseMessageTransfer_Customer_Service();
                        break;
                }

                (responseMessage as ResponseMessageBase).FillEntityWithXml(doc);//如果不是ResponseMessageBase会抛出异常
                return responseMessage;
            }
            catch (Exception ex)
            {
                throw new MessageHandlerException("ResponseMessageBase.CreateFromResponseXml<T>过程发生异常！" + ex.Message, ex);
            }
        }
    }
}
