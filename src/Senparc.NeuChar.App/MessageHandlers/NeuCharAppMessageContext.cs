﻿#pragma warning disable 1591
using Senparc.NeuChar.Context;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Senparc.NeuChar.App.MessageHandlers
{
    public class NeuCharAppMessageContext : MessageContext<RequestMessageNeuChar, SuccessResponseMessage>
    {
        public NeuCharAppMessageContext()
        {
            base.MessageContextRemoved += NeuCharAppMessageContext_MessageContextRemoved;
        }

        public override RequestMessageNeuChar GetRequestEntityMappingResult(RequestMsgType requestMsgType, XDocument doc = null)
        {
            if (requestMsgType != RequestMsgType.NeuChar)
            {
                throw new MessageHandlerException("仅支持 NeuChar 类型请求");
            }

            var requestMessage = new RequestMessageNeuChar();
            return requestMessage;
        }

        public override SuccessResponseMessage GetResponseEntityMappingResult(ResponseMsgType responseMsgType, XDocument doc = null)
        {
            throw new NotImplementedException();//不需要记录上下文，所以这里 ResponseMessage 消息可以忽略
        }

        /// <summary>
        /// 当上下文过期，被移除时触发的时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void NeuCharAppMessageContext_MessageContextRemoved(object sender, Senparc.NeuChar.Context.WeixinContextRemovedEventArgs<RequestMessageNeuChar, SuccessResponseMessage> e)
        {
            /* 注意，这个事件不是实时触发的（当然你也可以专门写一个线程监控）
             * 为了提高效率，根据WeixinContext中的算法，这里的过期消息会在过期后下一条请求执行之前被清除
             */

            var messageContext = e.MessageContext as NeuCharAppMessageContext;
            if (messageContext == null)
            {
                return;//如果是正常的调用，messageContext不会为null
            }

            //TODO:这里根据需要执行消息过期时候的逻辑，下面的代码仅供参考

            //Log.InfoFormat("{0}的消息上下文已过期",e.OpenId);
            //api.SendMessage(e.OpenId, "由于长时间未搭理客服，您的客服状态已退出！");
        }
    }
}
