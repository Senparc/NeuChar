using Senparc.NeuChar.Context;
using Senparc.NeuChar.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Senparc.NeuChar.Tests.MessageHandlers
{
    public class TestMpMessageContext : MessageContext<IRequestMessageBase, IResponseMessageBase>
    {
        public override IRequestMessageBase GetRequestEntityMappingResult(RequestMsgType requestMsgType)
        {
            throw new NotImplementedException();
        }

        public override IResponseMessageBase GetResponseEntityMappingResult(ResponseMsgType responseMsgType)
        {
            throw new NotImplementedException();
        }
    }
}
