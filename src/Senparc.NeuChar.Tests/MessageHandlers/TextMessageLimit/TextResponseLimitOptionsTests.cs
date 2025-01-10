using Microsoft.VisualStudio.TestTools.UnitTesting;
using Senparc.CO2NET.Extensions;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Helpers;
using Senparc.NeuChar.MessageHandlers;
using Senparc.NeuChar.Tests;
using Senparc.NeuChar.Tests.MessageHandlers;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Senparc.NeuChar.MessageHandlers.Tests
{
    [TestClass()]
    public class TextResponseLimitOptionsTests : BaseTest
    {
        string textRequestXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xml>
    <ToUserName><![CDATA[gh_a96a4a619366]]></ToUserName>
    <FromUserName><![CDATA[oxRg0uLsnpHjb8o93uVnwMK_WAVw]]></FromUserName>
    <CreateTime>{1}</CreateTime>
    <MsgType><![CDATA[text]]></MsgType>
    <Content><![CDATA[{0}]]></Content>
    <MsgId>{2}</MsgId>
</xml>
";

        [TestMethod()]
        public async Task CheckOrSendCustomMessageTest()
        {
            string content = "OK";

            #region 不使用任何限制

            {
                //不使用任何限制
                var xmlText = textRequestXml.FormatWith(content, Senparc.CO2NET.Helpers.DateTimeHelper.GetUnixDateTime(SystemTime.Now), SystemTime.Now.Ticks.ToString());

                var postModel = new PostModel() { AppId = "AppId" };

                var messageHandler = new TestMpMessageHandler(XDocument.Parse(xmlText), postModel);

                await messageHandler.ExecuteAsync(new System.Threading.CancellationToken());

                Assert.IsNotNull(messageHandler.TextResponseMessage);
                var response = messageHandler.ResponseMessage as IResponseMessageText;
                Assert.IsNotNull(response);
                Assert.AreEqual(content, response.Content);
            }
            #endregion


            #region 使用限制

            var accessToken = "70_saoWvxrkwg7EHKyqBrJ-dWSMDsIltTqITlxeOHJCi6br9Bu2uMpMjmIQU_30TTHjMgq-Hvr1Ve1BdhI1pf88jyxPOTM8fJ8H7t3-bVjQRmhk75UKJcJYVeIxkAgAAEcAIAKKO";

            {
                content = new string('C', 2048);//正好符合条件，不会调用客服接口

                //不使用任何限制
                var xmlText = textRequestXml.FormatWith(content, Senparc.CO2NET.Helpers.DateTimeHelper.GetUnixDateTime(SystemTime.Now), SystemTime.Now.Ticks.ToString());

                var postModel = new PostModel() { AppId = "AppId" };

                var messageHandler = new TestMpMessageHandler(XDocument.Parse(xmlText), postModel);

                //设置限制
                messageHandler.TextResponseLimitOptions = new TextResponseLimitOptions(2048, accessToken);

                await messageHandler.ExecuteAsync(new System.Threading.CancellationToken());


                Assert.IsNotNull(messageHandler.TextResponseMessage);
                await Console.Out.WriteLineAsync(messageHandler.TextResponseMessage.ToJson(true));
                var response = messageHandler.ResponseMessage as ResponseMessageText;
                Assert.IsNotNull(response);
                Assert.AreEqual(content, response.Content);
            }

            {
                content = new string('C', 2050);//超长，分2次发：一条 2048 个“C”，一条 2 个“C”

                //不使用任何限制
                var xmlText = textRequestXml.FormatWith(content, Senparc.CO2NET.Helpers.DateTimeHelper.GetUnixDateTime(SystemTime.Now), SystemTime.Now.Ticks.ToString());

                var postModel = new PostModel() { AppId = "AppId" };

                var messageHandler = new TestMpMessageHandler(XDocument.Parse(xmlText), postModel);

                //设置限制
                messageHandler.TextResponseLimitOptions = new TextResponseLimitOptions(2048, accessToken);

                await messageHandler.ExecuteAsync(new System.Threading.CancellationToken());


                Assert.IsNotNull(messageHandler.TextResponseMessage);
                await Console.Out.WriteLineAsync(messageHandler.TextResponseMessage.ToJson(true));
                var response = messageHandler.ResponseMessage as SuccessResponseMessageBase;
                Assert.IsNotNull(response);
            }

            {
                content = new string('盛', 683);//超长，分2次发
                content += "B派派";//最后的“派”不会被包含在1024范围内，另外发送

                //不使用任何限制
                var xmlText = textRequestXml.FormatWith(content, 
                                Senparc.CO2NET.Helpers.DateTimeHelper.GetUnixDateTime(SystemTime.Now), 
                                SystemTime.Now.Ticks.ToString());

                var postModel = new PostModel() { AppId = "AppId" };

                var messageHandler = new TestMpMessageHandler(XDocument.Parse(xmlText), postModel);

                //设置限制
                messageHandler.TextResponseLimitOptions = new TextResponseLimitOptions(2048, accessToken);

                await messageHandler.ExecuteAsync(new System.Threading.CancellationToken());


                Assert.IsNotNull(messageHandler.TextResponseMessage);
                await Console.Out.WriteLineAsync(messageHandler.TextResponseMessage.ToJson(true));
                var response = messageHandler.ResponseMessage as SuccessResponseMessageBase;
                Assert.IsNotNull(response);
            }

            #endregion
        }
    }
}