using Senparc.NeuChar.Context;
using Senparc.NeuChar.Entities;
using Senparc.Weixin.MP.Entities;
using System;
using System.Xml.Linq;

namespace Senparc.NeuChar.Tests.Context
{
    public class CustomMessageContext : MessageContext<RequestMessageBase, ResponseMessageBase>
    {
        public CustomMessageContext()
        {
            base.MessageContextRemoved += CustomMessageContext_MessageContextRemoved;
        }

        public override RequestMessageBase GetRequestEntityMappingResult(RequestMsgType requestMsgType, XDocument doc = null)
        {
            RequestMessageBase requestMessage;
            switch (requestMsgType)
            {
                case RequestMsgType.Text:
                    requestMessage = new RequestMessageText();//借用MP的对象，也可以在单元测试中新建类
                    break;
                case RequestMsgType.Event:
                    //判断Event类型
                    switch (doc.Root.Element("Event").Value.ToUpper())
                    {
                        case "SUBSCRIBE"://订阅（关注）
                            requestMessage = new RequestMessageEvent_Subscribe();
                            break;
                        default:
                            requestMessage = new RequestMessageUnknownType();
                            break;
                    }
                    break;
                default:
                    throw new Exception("未定义此类型：" + requestMsgType);
            }

            return requestMessage;
        }

        public override ResponseMessageBase GetResponseEntityMappingResult(ResponseMsgType responseMsgType, XDocument doc = null)
        {
            ResponseMessageBase responseMessage = null;
            switch (responseMsgType)
            {
                case ResponseMsgType.Text:
                    responseMessage = new ResponseMessageText();
                    break;
                default:
                    throw new Exception("未定义此类型：" + responseMsgType);
            }

            return responseMessage;
        }


        /// <summary>
        /// 当上下文过期，被移除时触发的时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CustomMessageContext_MessageContextRemoved(object sender, Senparc.NeuChar.Context.WeixinContextRemovedEventArgs<RequestMessageBase, ResponseMessageBase> e)
        {
            /* 注意，这个事件不是实时触发的（当然你也可以专门写一个线程监控）
             * 为了提高效率，根据WeixinContext中的算法，这里的过期消息会在过期后下一条请求执行之前被清除
             */

            var messageContext = e.MessageContext as CustomMessageContext;
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
