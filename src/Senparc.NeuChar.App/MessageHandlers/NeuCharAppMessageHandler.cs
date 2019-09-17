#region Apache License Version 2.0
/*----------------------------------------------------------------

Copyright 2019 Suzhou Senparc Network Technology Co.,Ltd.

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
    Copyright (C) 2019 Senparc
    
    文件名：NeuCharAppMessageHandler.cs
    文件功能描述：此 MessageHandler 仅提供 NeuChar 平台对接使用，无其他功能
    
    
    创建标识：Senparc - 20181129

    修改标识：Senparc - 20190914
    修改描述：v0.8.0 提供支持分布式缓存的消息上下文（MessageContext）
----------------------------------------------------------------*/

using Senparc.CO2NET.Cache;
using Senparc.CO2NET.Helpers;
using Senparc.NeuChar.ApiHandlers;
using Senparc.NeuChar.Context;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Exceptions;
using Senparc.NeuChar.Helpers;
using Senparc.NeuChar.MessageHandlers;
using System;
using System.IO;
using System.Xml.Linq;

namespace Senparc.NeuChar.App.MessageHandlers
{

    /// <summary>
    /// 此 MessageHandler 仅提供 NeuChar 平台对接使用，无其他功能
    /// </summary>
    public class NeuCharAppMessageHandler : MessageHandler<NeuCharAppMessageContext, RequestMessageNeuChar, SuccessResponseMessage>
    {

        #region 私有方法

        /// <summary>
        /// 标记为已重复消息
        /// </summary>
        private void MarkRepeatedMessage()
        {
            CancelExcute = true;//重复消息，取消执行
            MessageIsRepeated = true;
        }

        #endregion


        public NeuCharAppMessageHandler(Stream inputStream, EncryptPostModel postModel)
            : base(inputStream, postModel, 10)
        {

        }

        public override GlobalMessageContext<NeuCharAppMessageContext, RequestMessageNeuChar, SuccessResponseMessage> GlobalMessageContext => throw new NotImplementedException();

        public override MessageEntityEnlightener MessageEntityEnlightener => NeuCharAppMessageEntityEnlightener.Instance;

        public override ApiEnlightener ApiEnlightener => throw new NotImplementedException();

        public override XDocument ResponseDocument => ResponseMessage != null ? NeuChar.Helpers.EntityHelper.ConvertEntityToXml(ResponseMessage as ResponseMessageBase) : null;


        public override XDocument FinalResponseDocument => null;

        public override void BuildResponseMessage()
        {
            //不做处理
        }

        public override XDocument Init(XDocument postDataDocument, IEncryptPostModel postModel)
        {
            //进行加密判断并处理
            var postDataStr = postDataDocument.ToString();

            XDocument decryptDoc = postDataDocument;

            //if (postModel != null && postDataDocument.Root.Element("Encrypt") != null && !string.IsNullOrEmpty(postDataDocument.Root.Element("Encrypt").Value))
            //{
            //    //使用了加密
            //    UsingEcryptMessage = true;
            //    EcryptRequestDocument = postDataDocument;

            //    WXBizMsgCrypt msgCrype = new WXBizMsgCrypt(_postModel.Token, _postModel.EncodingAESKey, _postModel.AppId);
            //    string msgXml = null;
            //    var result = msgCrype.DecryptMsg(_postModel.Msg_Signature, _postModel.Timestamp, _postModel.Nonce, postDataStr, ref msgXml);

            //    //判断result类型
            //    if (result != 0)
            //    {
            //        //验证没有通过，取消执行
            //        CancelExcute = true;
            //        return null;
            //    }

            //    if (postDataDocument.Root.Element("FromUserName") != null && !string.IsNullOrEmpty(postDataDocument.Root.Element("FromUserName").Value))
            //    {
            //        //TODO：使用了兼容模式，进行验证即可
            //        UsingCompatibilityModelEcryptMessage = true;
            //    }

            //    decryptDoc = XDocument.Parse(msgXml);//完成解密
            //}

            var msgType = MsgTypeHelper.GetRequestMsgType(decryptDoc);
            if (msgType != RequestMsgType.NeuChar)
            {
                throw new MessageHandlerException("仅支持 NeuChar 类型请求");
            }

            RequestMessage = new RequestMessageNeuChar();
            if (UsingEcryptMessage)
            {
                RequestMessage.Encrypt = postDataDocument.Root.Element("Encrypt").Value;
            }

            //TODO:分布式系统中本地的上下文会有同步问题，需要同步使用远程的储存
            if (MessageContextGlobalConfig.UseMessageContext)
            {
                //var omit = OmitRepeatedMessageFunc == null || OmitRepeatedMessageFunc(RequestMessage);

                var cache = CacheStrategyFactory.GetObjectCacheStrategyInstance();
                using (cache.BeginCacheLock(MessageContextGlobalConfig.MESSAGE_CONTENT_OMIT_REPEAT_LOCK_NAME, "NeuCharAppMessageHandler"))//使用分布式缓存
                {
                    #region 消息去重

                    if (/*omit &&*/
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
