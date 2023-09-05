using Microsoft.VisualStudio.TestTools.UnitTesting;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Tests.MessageHandlers;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.Work.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MP = Senparc.Weixin.MP.Entities;
using Work = Senparc.Weixin.Work.Entities;

namespace Senparc.NeuChar.Entities.Tests
{
    [TestClass()]
    public class ResponseMessageBaseTests
    {
        [TestMethod()]
        public void CreateFromRequestMessageTest()
        {
            //MP
            IRequestMessageBase requestMessage = new MP.RequestMessageText();
            IResponseMessageBase responseMessage = ResponseMessageBase.CreateFromRequestMessage<MP.ResponseMessageText>(requestMessage);
            Assert.IsNotNull(responseMessage);
            Assert.IsInstanceOfType(responseMessage, typeof(MP.ResponseMessageText));

            responseMessage = ResponseMessageBase.CreateFromRequestMessage<MP.ResponseMessageNews>(requestMessage);
            Assert.IsNotNull(responseMessage);
            Assert.IsInstanceOfType(responseMessage, typeof(MP.ResponseMessageNews));

            responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageNoResponse>(requestMessage);
            Assert.IsNotNull(responseMessage);
            Assert.IsInstanceOfType(responseMessage, typeof(ResponseMessageNoResponse));

            //Work
            requestMessage = new Work.RequestMessageText();
            responseMessage = ResponseMessageBase.CreateFromRequestMessage<WorkResponseMessageNoResponse>(requestMessage);
            Assert.IsNotNull(responseMessage);
            Assert.IsInstanceOfType(responseMessage, typeof(WorkResponseMessageNoResponse));
        }
    }
}