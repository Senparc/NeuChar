using Microsoft.VisualStudio.TestTools.UnitTesting;
using Senparc.CO2NET.Cache;
using Senparc.CO2NET.Cache.Redis;
using Senparc.CO2NET.Extensions;
using Senparc.NeuChar.Context;
using Senparc.NeuChar.Entities;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Senparc.NeuChar.Tests.Context
{
    /// <summary>
    /// 消息上下文单元测试
    /// </summary>
    [TestClass]
    public class MessageContextTests : BaseTest
    {

        string textRequestXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<xml>
    <ToUserName><![CDATA[gh_a96a4a619366]]></ToUserName>
    <FromUserName><![CDATA[olPjZjsXuQPJoV0HlruZkNzKc91E]]></FromUserName>
    <CreateTime>{1}</CreateTime>
    <MsgType><![CDATA[text]]></MsgType>
    <Content><![CDATA[{0}]]></Content>
    <MsgId>{2}</MsgId>
</xml>
";

        PostModel postModel = new PostModel()
        {
            Token = "Token"
        };

        public MessageContextTests()
        {
            //注册
            var redisConfigurationStr = "localhost:6379,defaultDatabase=3";
            Senparc.CO2NET.Cache.Redis.Register.SetConfigurationOption(redisConfigurationStr);
            Senparc.CO2NET.Cache.Redis.Register.UseKeyValueRedisNow();//键值对缓存策略（推荐）
        }

        #region DistributedCacheTest


        public void DistributedCacheTest(Func<IBaseObjectCacheStrategy> cacheStrategy)
        {
            //强制使用本地缓存
            CacheStrategyFactory.RegisterObjectCacheStrategy(cacheStrategy);

            Console.WriteLine($"当前使用缓存：{CacheStrategyFactory.GetObjectCacheStrategyInstance().GetType().FullName}");

            //清空缓存
            var globalMessageContext = new GlobalMessageContext<CustomMessageContext, RequestMessageBase, ResponseMessageBase>();
            globalMessageContext.Restore();


            //第一次请求
            var dt1 = SystemTime.Now;
            var doc = XDocument.Parse(textRequestXml.FormatWith("TNT2", CO2NET.Helpers.DateTimeHelper.GetUnixDateTime(SystemTime.Now.UtcDateTime), SystemTime.Now.Ticks));
            var messageHandler = new CustomMessageHandler(doc, postModel);

            Assert.AreEqual(1, messageHandler.CurrentMessageContext.RequestMessages.Count);//初始化之后，RequestMessage 已经被记录到上下文中
            Assert.AreEqual(0, messageHandler.CurrentMessageContext.ResponseMessages.Count);

            messageHandler.Execute();
            Console.WriteLine($"第 1 次请求耗时：{SystemTime.NowDiff(dt1).TotalMilliseconds} ms");

            Assert.AreEqual(1, messageHandler.CurrentMessageContext.RequestMessages.Count);
            Assert.AreEqual(1, messageHandler.CurrentMessageContext.ResponseMessages.Count);
            Console.WriteLine(messageHandler.CurrentMessageContext.ResponseMessages.Last().GetType());
            Console.WriteLine(messageHandler.CurrentMessageContext.ResponseMessages.Last().ToJson());

            var lastResponseMessage = messageHandler.CurrentMessageContext.ResponseMessages.Last() as ResponseMessageText;
            Assert.IsNotNull(lastResponseMessage);
            Assert.AreEqual("来自单元测试:TNT2", lastResponseMessage.Content);


            //第二次请求
            var dt2 = SystemTime.Now;
            doc = XDocument.Parse(textRequestXml.FormatWith("TNT3", CO2NET.Helpers.DateTimeHelper.GetUnixDateTime(SystemTime.Now.UtcDateTime), SystemTime.Now.Ticks));
            messageHandler = new CustomMessageHandler(doc, postModel);
            Assert.AreEqual(2, messageHandler.CurrentMessageContext.RequestMessages.Count);
            Assert.AreEqual(1, messageHandler.CurrentMessageContext.ResponseMessages.Count);

            messageHandler.Execute();
            Console.WriteLine($"第 2 次请求耗时：{SystemTime.NowDiff(dt2).TotalMilliseconds} ms");

            Assert.AreEqual(2, messageHandler.CurrentMessageContext.RequestMessages.Count);
            Assert.AreEqual(2, messageHandler.CurrentMessageContext.ResponseMessages.Count);

            lastResponseMessage = messageHandler.CurrentMessageContext.ResponseMessages.Last() as ResponseMessageText;
            Assert.IsNotNull(lastResponseMessage);
            Assert.AreEqual("来自单元测试:TNT3", lastResponseMessage.Content);


            //测试去重
            var dt3 = SystemTime.Now;
            messageHandler = new CustomMessageHandler(doc, postModel);//使用和上次同样的请求
            Assert.AreEqual(2, messageHandler.CurrentMessageContext.RequestMessages.Count);
            Assert.AreEqual(2, messageHandler.CurrentMessageContext.ResponseMessages.Count);

            messageHandler.Execute();
            Console.WriteLine($"第 3 次请求耗时：{SystemTime.NowDiff(dt3).TotalMilliseconds} ms");
            //没有变化
            Assert.AreEqual(2, messageHandler.CurrentMessageContext.RequestMessages.Count);
            Assert.AreEqual(2, messageHandler.CurrentMessageContext.ResponseMessages.Count);
            lastResponseMessage = messageHandler.CurrentMessageContext.ResponseMessages.Last() as ResponseMessageText;
            Assert.IsNotNull(lastResponseMessage);
            Assert.AreEqual("来自单元测试:TNT3", lastResponseMessage.Content);


            //测试最大纪录储存

            Console.WriteLine("==== 循环测试开始 ====");
            for (int i = 0; i < 15; i++)
            {
                var dt4 = SystemTime.Now;
                doc = XDocument.Parse(textRequestXml.FormatWith($"循环测试-{i}", CO2NET.Helpers.DateTimeHelper.GetUnixDateTime(SystemTime.Now.UtcDateTime) + i, SystemTime.Now.Ticks));
                var maxRecordCount = 10;
                messageHandler = new CustomMessageHandler(doc, postModel, maxRecordCount);//使用和上次同样的请求
                //messageHandler.GlobalMessageContext.MaxRecordCount = 10;//在这里设置的话，Request已经插入了，无法及时触发删除多余消息的过程
                messageHandler.Execute();

                Assert.AreEqual(i < 7 ? i + 3 : 10, messageHandler.CurrentMessageContext.RequestMessages.Count);
                Assert.AreEqual(i < 7 ? i + 3 : 10, messageHandler.CurrentMessageContext.ResponseMessages.Count);

                Console.WriteLine($"第 {i + 1} 次循环测试请求耗时：{SystemTime.NowDiff(dt4).TotalMilliseconds} ms");
            }
            Console.WriteLine("==== 循环测试结束 ====");


            //清空
            messageHandler.GlobalMessageContext.Restore();
            Assert.AreEqual(0, messageHandler.CurrentMessageContext.RequestMessages.Count);
            Assert.AreEqual(0, messageHandler.CurrentMessageContext.ResponseMessages.Count);

            Console.WriteLine();
        }

        [TestMethod]
        public void LocalCacheTest()
        {
            DistributedCacheTest(() => CO2NET.Cache.LocalObjectCacheStrategy.Instance);
        }

        [TestMethod]
        public void RedisCacheTest()
        {
            DistributedCacheTest(() => CO2NET.Cache.Redis.RedisObjectCacheStrategy.Instance);
        }
        #endregion

        /// <summary>
        /// 全局参数设置
        /// </summary>
        [TestMethod]
        public void GlobalSettingTest()
        {
            var doc = XDocument.Parse(textRequestXml.FormatWith("GlobalSettingTest", CO2NET.Helpers.DateTimeHelper.GetUnixDateTime(SystemTime.Now.UtcDateTime), SystemTime.Now.Ticks));
            var recordCount = MessageContextGlobalConfig.MaxRecordCount + 1;
            var messageHandler = new CustomMessageHandler(doc, postModel, recordCount);

            Assert.AreEqual(MessageContextGlobalConfig.ExpireMinutes, messageHandler.GlobalMessageContext.ExpireMinutes);
            Assert.AreEqual(MessageContextGlobalConfig.MaxRecordCount + 1, messageHandler.GlobalMessageContext.MaxRecordCount);
            Console.WriteLine($"MessageContextGlobalConfig.ExpireMinutes:{MessageContextGlobalConfig.ExpireMinutes}");
            Console.WriteLine($"MessageContextGlobalConfig.MaxRecordCount:{MessageContextGlobalConfig.MaxRecordCount}");

            //小范围参数设置不影响全局参数
            messageHandler.GlobalMessageContext.ExpireMinutes = 99;
            messageHandler.GlobalMessageContext.MaxRecordCount = 88;

            Assert.AreEqual(99, messageHandler.GlobalMessageContext.ExpireMinutes);
            Assert.AreEqual(88, messageHandler.GlobalMessageContext.MaxRecordCount);

            //全局参数修改后，所有对象都会被更新
            MessageContextGlobalConfig.ExpireMinutes = 199;
            MessageContextGlobalConfig.MaxRecordCount = 188;

            Assert.AreEqual(199, MessageContextGlobalConfig.ExpireMinutes);
            Assert.AreEqual(188, MessageContextGlobalConfig.MaxRecordCount);
            Assert.AreEqual(199, messageHandler.GlobalMessageContext.ExpireMinutes);
            Assert.AreEqual(188, messageHandler.GlobalMessageContext.MaxRecordCount);

            //重置参数
            MessageContextGlobalConfig.Restore();
            Assert.AreEqual(30, MessageContextGlobalConfig.ExpireMinutes);
            Assert.AreEqual(20, MessageContextGlobalConfig.MaxRecordCount);

            Assert.AreEqual(30, messageHandler.GlobalMessageContext.ExpireMinutes);
            Assert.AreEqual(20, messageHandler.GlobalMessageContext.MaxRecordCount);

        }
    }
}
