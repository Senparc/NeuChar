using Senparc.NeuChar.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Senparc.NeuChar.Tests.MessageHandlers;
using System.Xml.Linq;
using Senparc.Weixin.MP.Entities.Request;

namespace Senparc.NeuChar.Entities.Tests
{
    [TestClass()]
    public class RequestMessageBaseTests
    {
        [TestMethod()]
        public void RequestMessageBaseTest()
        {
            var xmlText = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xml>
  <ToUserName><![CDATA[gh_0fe614101343]]></ToUserName>
<FromUserName><![CDATA[oxRg0uIHgyqH-Lf1vNp2eIdTswu8]]></FromUserName>
<CreateTime>1688550197</CreateTime>
<MsgType><![CDATA[text]]></MsgType>
<Content><![CDATA[S]]></Content>
<MsgId>24174226108955567</MsgId>
</xml>";
            var postModel = new PostModel() { AppId = "" };
            var messageHandler = new TestMpMessageHandler(XDocument.Parse(xmlText), postModel);


            //TODO: 更新微信 SDK 后继续测试

        }
    }
}

