#region Apache License Version 2.0
/*----------------------------------------------------------------

Copyright 2021 Suzhou Senparc Network Technology Co.,Ltd.

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
    Copyright (C) 2021 Senparc
    
    文件名：NeuCharAppMessageHandler.cs
    文件功能描述：此 MessageHandler 仅提供 NeuChar 平台对接使用，无其他功能
    
    
    创建标识：Senparc - 20181129

    修改标识：Senparc - 20190914
    修改描述：v0.8.0 提供支持分布式缓存的消息上下文（MessageContext）
    
    修改标识：Senparc - 20190928
    修改描述：v0.6.101 NeuCharAppMessageHandler 改用基类的上下文处理能力
----------------------------------------------------------------*/

#pragma warning disable 1591
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
using System.Threading;
using System.Threading.Tasks;
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

        public override XDocument Init(XDocument postDataDocument, IEncryptPostModel postModel)
        {
            //进行加密判断并处理
            var postDataStr = postDataDocument.ToString();

            XDocument decryptDoc = postDataDocument;

            //if (postModel != null && postDataDocument.Root.Element("Encrypt") != null && !string.IsNullOrEmpty(postDataDocument.Root.Element("Encrypt").Value))
            //{
            //    //使用了加密
            //    UsingEncryptMessage = true;
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
            //        UsingCompatibilityModelEncryptMessage = true;
            //    }

            //    decryptDoc = XDocument.Parse(msgXml);//完成解密
            //}

            var msgType = MsgTypeHelper.GetRequestMsgType(decryptDoc);
            if (msgType != RequestMsgType.NeuChar)
            {
                throw new MessageHandlerException("仅支持 NeuChar 类型请求");
            }

            RequestMessage = new RequestMessageNeuChar();
            if (UsingEncryptMessage)
            {
                RequestMessage.Encrypt = postDataDocument.Root.Element("Encrypt").Value;
            }

            return decryptDoc;

            //消息上下文记录将在 base.CommonInitialize() 中根据去重等条件判断后进行添加
        }

        public override Task BuildResponseMessageAsync(CancellationToken cancellationToken)
        {
            //不作处理
#if NET45
            return Task.Delay(0);
#else
           return Task.CompletedTask;
#endif
        }
    }
}
