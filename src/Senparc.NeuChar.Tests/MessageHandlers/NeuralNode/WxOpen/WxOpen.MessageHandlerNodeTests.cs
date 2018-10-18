using Microsoft.VisualStudio.TestTools.UnitTesting;
using Senparc.Weixin.MP.Entities.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Senparc.NeuChar.Tests.MessageHandlers.NeuralNode.WxOpen
{
    [TestClass]
    public class MessageHandlerNodeTests : BaseTest
    {

        [TestMethod]
        public void RenderResponseMessageNewsTest()
        {
            var xmlText = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xml>
  <ToUserName><![CDATA[gh_6e306981e699]]></ToUserName>
  <FromUserName><![CDATA[oeaTy0DgoGq-lyqvTauWVjbIVuP0]]></FromUserName>
  <CreateTime>1539828411</CreateTime>
  <MsgType><![CDATA[text]]></MsgType>
  <Content><![CDATA[N]]></Content>
  <MsgId>6613512667117942239</MsgId>
</xml>";

            var postModel = new PostModel() { AppId="" };

            var messageHandler = new TestMpMessageHandler(XDocument.Parse(xmlText), postModel);
            messageHandler.Execute();

            Assert.IsNotNull(messageHandler.TextResponseMessage);
            Console.WriteLine(messageHandler.TextResponseMessage);
        }

    }
}