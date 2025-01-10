using Senparc.NeuChar.Context;
using Senparc.NeuChar.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Senparc.NeuChar.Tests.MessageHandlers
{
    public class TestMpMessageContext : MessageContext<IRequestMessageBase, IResponseMessageBase>
    {
        public override IRequestMessageBase GetRequestEntityMappingResult(RequestMsgType requestMsgType, XDocument doc = null)
        {
            throw new NotImplementedException();
        }

        public override IResponseMessageBase GetResponseEntityMappingResult(ResponseMsgType responseMsgType, XDocument doc = null)
        {
            throw new NotImplementedException();
        }
    }
}
