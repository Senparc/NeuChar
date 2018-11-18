using Microsoft.VisualStudio.TestTools.UnitTesting;
using Senparc.Weixin.MP.Entities.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Senparc.NeuChar.Tests.MessageHandlers.NeuralNode.MP
{
    [TestClass]
    public class MessageHandlerNodeTests : BaseTest
    {

        [TestMethod]
        public void RenderResponseMessageNewsTest()
        {
            var xmlText = @"<?xml version=""1.0"" encoding=""utf-8""?>
 <xml>
  <ToUserName><![CDATA[gh_0fe614101343]]></ToUserName>
  <FromUserName><![CDATA[oxRg0uLsnpHjb8o93uVnwMK_WAVw]]></FromUserName>
  <CreateTime>1539684529</CreateTime>
  <MsgType><![CDATA[event]]></MsgType>
  <Event><![CDATA[CLICK]]></Event>
  <EventKey><![CDATA[NEUCHAR|43E8BCD9]]></EventKey>
</xml>";

            var postModel = new PostModel() { AppId= "AppId" };

            var messageHandler = new TestMpMessageHandler(XDocument.Parse(xmlText), postModel);
            messageHandler.Execute();

            Assert.IsNotNull(messageHandler.TextResponseMessage);
            Console.WriteLine(messageHandler.TextResponseMessage);
        }

        [TestMethod]
        public void SaveLogTest()
        {
            var xmlText = @"<?xml version=""1.0"" encoding=""utf-8""?>
 <xml>
  <ToUserName><![CDATA[gh_0fe614101343]]></ToUserName>
  <FromUserName><![CDATA[oxRg0uLsnpHjb8o93uVnwMK_WAVw]]></FromUserName>
  <CreateTime>1539684529</CreateTime>
  <MsgType><![CDATA[event]]></MsgType>
  <Event><![CDATA[CLICK]]></Event>
  <EventKey><![CDATA[NEUCHAR|43E8BCD9]]></EventKey>
</xml>";

            var postModel = new PostModel() { AppId = "AppId" };

            var messageHandler = new TestMpMessageHandler(XDocument.Parse(xmlText), postModel);

            messageHandler.SaveRequestMessageLog();
            messageHandler.Execute();
            messageHandler.SaveResponseMessageLog();

            var dt1 = SystemTime.Now;
            while ((SystemTime.Now - dt1).TotalMilliseconds < 800)
            {
                //等待队列执行
            }

            Assert.IsNotNull(messageHandler.TextResponseMessage);
            Console.WriteLine(messageHandler.TextResponseMessage);
        }
    }
}