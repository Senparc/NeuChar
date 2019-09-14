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
        public override GlobalMessageContext<CustomMessageContext, RequestMessageBase, ResponseMessageBase> GlobalMessageContext => new GlobalMessageContext<CustomMessageContext, RequestMessageBase, ResponseMessageBase>();

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

        private Action<IRequestMessageBase, CustomMessageHandler/* MessageHandler<CustomMessageContext, IRequestMessageBase, IResponseMessageBase>*/> specialJudgeAction = (lastMessage, messageHandler) =>
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
                            messageHandler.MarkRepeatedMessage();//标记为已重复
                        }
                        break;
                    default:
                        break;
                }
            }
        };

        public CustomMessageHandler(XDocument postDataDocument, IEncryptPostModel postModel, int maxRecordCount = 0) 
            : base(postDataDocument, postModel, maxRecordCount)
        {

        }

        //TODO:独立添加去重过程，并且可扩展


        public override XDocument Init(XDocument requestDocument, IEncryptPostModel postModel)
        {
            XDocument decryptDoc = requestDocument;
            RequestMessage = RequestMessageFactory.GetRequestEntity(decryptDoc) as RequestMessageBase;

            //去重    TODO：分离独立方法
            if (MessageContextGlobalConfig.UseMessageContext)
            {
                var omit = OmitRepeatedMessageFunc == null || OmitRepeatedMessageFunc(RequestMessage);

                var cache = CacheStrategyFactory.GetObjectCacheStrategyInstance();
                using (cache.BeginCacheLock(MessageContextGlobalConfig.MESSAGE_CONTENT_OMIT_REPEAT_LOCK_NAME, "MessageHandler-Init"))//使用分布式锁
                {
                    #region 消息去重

                    if (omit &&
                        OmitRepeatedMessage &&
                        CurrentMessageContext.RequestMessages.Count > 0
                         //&& !(RequestMessage is RequestMessageEvent_Merchant_Order)批量订单的MsgId可能会相同
                         )
                    {
                        //lastMessage必定有值（除非极端小的过期时间条件下，几乎不可能发生）
                        var lastMessage = CurrentMessageContext.RequestMessages[CurrentMessageContext.RequestMessages.Count - 1];

                        if (
                            //使用MsgId去重
                            (lastMessage.MsgId != 0 && lastMessage.MsgId == RequestMessage.MsgId) ||
                            //使用CreateTime去重（OpenId对象已经是同一个）
                            (lastMessage.MsgId == RequestMessage.MsgId &&
                                 lastMessage.CreateTime == RequestMessage.CreateTime &&
                                 lastMessage.MsgType == RequestMessage.MsgType)
                            )
                        {
                            MarkRepeatedMessage();//标记为已重复
                        }

                        //判断特殊事件
                        if (!MessageIsRepeated &&
                            lastMessage is RequestMessageEventBase &&
                            RequestMessage is RequestMessageEventBase &&
                            (lastMessage as RequestMessageEventBase).Event == (RequestMessage as RequestMessageEventBase).Event
                            )
                        {
                            var lastEventMessage = lastMessage as RequestMessageEventBase;
                            var currentEventMessage = RequestMessage as RequestMessageEventBase;
                            switch (lastEventMessage.Event)
                            {

                                case Event.user_get_card://领取事件推送
                                    //文档：https://mp.weixin.qq.com/wiki?t=resource/res_main&id=mp1451025274
                                    //问题反馈：https://github.com/JeffreySu/WeiXinMPSDK/issues/1106
                                    var lastGetUserCardMessage = lastMessage as RequestMessageEvent_User_Get_Card;
                                    var currentGetUserCardMessage = RequestMessage as RequestMessageEvent_User_Get_Card;
                                    if (lastGetUserCardMessage.UserCardCode == currentGetUserCardMessage.UserCardCode &&
                                        lastGetUserCardMessage.CardId == currentGetUserCardMessage.CardId)
                                    {
                                        MarkRepeatedMessage();//标记为已重复
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    #endregion

                    //在消息没有被去重的情况下记录上下文
                    if (!MessageIsRepeated)
                    {
                        GlobalMessageContext.InsertMessage(RequestMessage);
                    }
                }
            }

            return decryptDoc;
        }
    }
}
