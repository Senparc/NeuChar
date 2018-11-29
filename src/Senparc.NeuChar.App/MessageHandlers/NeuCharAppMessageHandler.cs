using Senparc.NeuChar.ApiHandlers;
using Senparc.NeuChar.Context;
using Senparc.NeuChar.Entities;
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
        public NeuCharAppMessageHandler(Stream inputStream,EncryptPostModel postModel)
            :base(inputStream, postModel)
        {

        }

        public override GlobalMessageContext<NeuCharAppMessageContext, RequestMessageNeuChar, SuccessResponseMessage> GlobalMessageContext => throw new NotImplementedException();

        public override MessageEntityEnlightener MessageEntityEnlightener => throw new NotImplementedException();

        public override ApiEnlightener ApiEnlightener => throw new NotImplementedException();

        public override XDocument ResponseDocument => throw new NotImplementedException();

        public override XDocument FinalResponseDocument => throw new NotImplementedException();

        public override void BuildResponseMessage()
        {
            throw new NotImplementedException();
        }

        public override XDocument Init(XDocument requestDocument, IEncryptPostModel postModel)
        {
            throw new NotImplementedException();
        }
    }
}
