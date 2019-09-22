using Senparc.CO2NET.Cache;
using Senparc.NeuChar.ApiHandlers;
using Senparc.NeuChar.Context;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Helpers;
using Senparc.NeuChar.MessageHandlers;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.MessageHandlers;
using Senparc.Weixin.Tencent;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Senparc.NeuChar.Tests.Context
{
    public class CustomMessageHandler : MessageHandler<CustomMessageContext, RequestMessageBase, ResponseMessageBase>
    {
        public override MessageEntityEnlightener MessageEntityEnlightener => MpMessageEntityEnlightener.Instance;

        public override ApiEnlightener ApiEnlightener => MpApiEnlightener.Instance;

        public override XDocument ResponseDocument => ResponseMessage != null ? EntityHelper.ConvertEntityToXml(ResponseMessage as ResponseMessageBase) : null;


        public override XDocument FinalResponseDocument
        {
            get
            {
                return ResponseDocument;//明文
            }
        }

        public override void BuildResponseMessage()
        {
            var requestMessage = RequestMessage as RequestMessageText;
            var responseMessge = this.CreateResponseMessage<ResponseMessageText>();
            responseMessge.Content = $"来自单元测试:{requestMessage.Content}";
            ResponseMessage = responseMessge;
        }

        /// <summary>
        /// 动态去重判断委托，仅当返回值为false时，不使用消息去重功能
        /// </summary>
        public Func<IRequestMessageBase, bool> OmitRepeatedMessageFunc { get; set; } = null;


        /// <summary>
        /// 标记为已重复消息
        /// </summary>
        public virtual void MarkRepeatedMessage()
        {
            CancelExcute = true;//重复消息，取消执行
            MessageIsRepeated = true;
        }

        public CustomMessageHandler(XDocument postDataDocument, IEncryptPostModel postModel, int maxRecordCount = 0)
            : base(postDataDocument, postModel, maxRecordCount)
        {

        }

        public override XDocument Init(XDocument requestDocument, IEncryptPostModel postModel)
        {
            XDocument decryptDoc = requestDocument;
            RequestMessage = RequestMessageFactory.GetRequestEntity(decryptDoc) as RequestMessageBase;

            base.SpecialDeduplicationAction = (lastMessage, messageHandler) =>
             {
                 //判断特殊事件
                 if (!messageHandler.MessageIsRepeated &&
                      lastMessage is RequestMessageEventBase &&
                      messageHandler.RequestMessage is RequestMessageEventBase &&
                      (lastMessage as RequestMessageEventBase).Event == (messageHandler.RequestMessage as RequestMessageEventBase).Event
                      )
                 {
                     var lastEventMessage = lastMessage as RequestMessageEventBase;
                     var currentEventMessage = messageHandler.RequestMessage as RequestMessageEventBase;
                     switch (lastEventMessage.Event)
                     {

                         case Event.user_get_card://领取事件推送
                                                  //文档：https://mp.weixin.qq.com/wiki?t=resource/res_main&id=mp1451025274
                                                  //问题反馈：https://github.com/JeffreySu/WeiXinMPSDK/issues/1106
                             var lastGetUserCardMessage = lastMessage as RequestMessageEvent_User_Get_Card;
                             var currentGetUserCardMessage = messageHandler.RequestMessage as RequestMessageEvent_User_Get_Card;
                             if (lastGetUserCardMessage.UserCardCode == currentGetUserCardMessage.UserCardCode &&
                                 lastGetUserCardMessage.CardId == currentGetUserCardMessage.CardId)
                             {
                                 return true;
                             }
                             break;
                         default:
                             break;
                     }
                 }
                 return false;
             };

            //消息去重的基本方法已经在基类 CommonInitialize() 中实现

            return decryptDoc;
        }

        public override void OnExecuting()
        {
            var currentMessageContext = base.GetCurrentMessageContext();
            if (currentMessageContext.StorageData == null || (currentMessageContext.StorageData is int))
            {
                currentMessageContext.StorageData = (int)0;
                GlobalMessageContext.UpdateMessageContext(currentMessageContext);//储存到缓存
            }
            base.OnExecuting();
        }

        public override void OnExecuted()
        {
            base.OnExecuted();

            var currentMessageContext = base.GetCurrentMessageContext();
            currentMessageContext.StorageData = ((int)currentMessageContext.StorageData) + 1;
            GlobalMessageContext.UpdateMessageContext(currentMessageContext);//储存到缓存
        }
    }
}
