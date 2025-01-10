using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Senparc.NeuChar.Context;
using Senparc.NeuChar.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Senparc.NeuChar.Tests.Context.JsonConverters
{
    [TestClass]
    public class MessageContextJsonConverterTests
    {
        [TestMethod]
        public void ConvertTest()
        {
            var jsonStr = @"{
  ""UserName"": ""FromUserName(OpenId)"",
  ""LastActiveTime"": ""2019-09-24T15:17:54.3197922+00:00"",
  ""ThisActiveTime"": ""2019-09-23T07:17:59.6691421+00:00"",
  ""RequestMessages"": [
    {
      ""MsgType"": 0,
      ""Content"": ""天气"",
      ""bizmsgmenuid"": """",
      ""MsgId"": 637047906563632900,
      ""Encrypt"": """",
      ""ToUserName"": ""ToUserName"",
      ""FromUserName"": ""FromUserName(OpenId)"",
      ""CreateTime"": ""2019-09-22T23:14:06+08:00""
    },
    {
      ""MsgType"": 0,
      ""Content"": ""天气"",
      ""bizmsgmenuid"": """",
      ""MsgId"": 637047906563642900,
      ""Encrypt"": """",
      ""ToUserName"": ""ToUserName"",
      ""FromUserName"": ""FromUserName(OpenId)"",
      ""CreateTime"": ""2019-09-22T23:14:08+08:00""
    },
    {
      ""MsgType"": 0,
      ""Content"": ""天气"",
      ""bizmsgmenuid"": """",
      ""MsgId"": 637047906563652900,
      ""Encrypt"": """",
      ""ToUserName"": ""ToUserName"",
      ""FromUserName"": ""FromUserName(OpenId)"",
      ""CreateTime"": ""2019-09-22T23:14:08+08:00""
    },
    {
      ""MsgType"": 0,
      ""Content"": ""天气"",
      ""bizmsgmenuid"": """",
      ""MsgId"": 637047906563662800,
      ""Encrypt"": """",
      ""ToUserName"": ""ToUserName"",
      ""FromUserName"": ""FromUserName(OpenId)"",
      ""CreateTime"": ""2019-09-22T23:14:08+08:00""
    },
    {
      ""MsgType"": 0,
      ""Content"": ""天气"",
      ""bizmsgmenuid"": """",
      ""MsgId"": 637047906563672800,
      ""Encrypt"": """",
      ""ToUserName"": ""ToUserName"",
      ""FromUserName"": ""FromUserName(OpenId)"",
      ""CreateTime"": ""2019-09-22T23:14:08+08:00""
    },
    {
      ""MsgType"": 0,
      ""Content"": ""天气"",
      ""bizmsgmenuid"": """",
      ""MsgId"": 637047906563682800,
      ""Encrypt"": """",
      ""ToUserName"": ""ToUserName"",
      ""FromUserName"": ""FromUserName(OpenId)"",
      ""CreateTime"": ""2019-09-22T23:14:10+08:00""
    },
    {
      ""MsgType"": 0,
      ""Content"": ""1234"",
      ""bizmsgmenuid"": """",
      ""MsgId"": 637047910625007590,
      ""Encrypt"": """",
      ""ToUserName"": ""ToUserName"",
      ""FromUserName"": ""FromUserName(OpenId)"",
      ""CreateTime"": ""2019-09-22T23:17:49+08:00""
    },
    {
      ""MsgType"": 0,
      ""Content"": ""1234"",
      ""bizmsgmenuid"": """",
      ""MsgId"": 637047910625017600,
      ""Encrypt"": """",
      ""ToUserName"": ""ToUserName"",
      ""FromUserName"": ""FromUserName(OpenId)"",
      ""CreateTime"": ""2019-09-22T23:17:53+08:00""
    },
    {
      ""MsgType"": 0,
      ""Content"": ""1234"",
      ""bizmsgmenuid"": """",
      ""MsgId"": 637047910625027600,
      ""Encrypt"": """",
      ""ToUserName"": ""ToUserName"",
      ""FromUserName"": ""FromUserName(OpenId)"",
      ""CreateTime"": ""2019-09-22T23:17:54+08:00""
    },
    {
      ""MsgType"": 0,
      ""Content"": ""天气"",
      ""bizmsgmenuid"": """",
      ""MsgId"": 637047910625037600,
      ""Encrypt"": """",
      ""ToUserName"": ""ToUserName"",
      ""FromUserName"": ""FromUserName(OpenId)"",
      ""CreateTime"": ""2019-09-22T23:17:59+08:00""
    }
  ],
  ""ResponseMessages"": [
    {
      ""MsgType"": 0,
      ""Content"": ""您刚才发送了文字信息：天气\r\n\r\n您刚才还发送了如下消息（10/0）：\r\n23:13:22 【Text】天气\r\n23:13:21 【Text】天气\r\n23:13:20 【Text】天气\r\n23:11:13 【Text】天气\r\n23:11:05 【Text】12345\r\n23:08:32 【Text】苏州\r\n23:07:06 【Text】天气\r\n23:04:51 【Text】111222\r\n23:03:31 【Text】111222\r\n\r\n\r\n如果您在3分钟内连续发送消息，记录将被自动保留（当前设置：最多记录10条）。过期后记录将会自动清除。\r\n\r\n\r\n您还可以发送【位置】【图片】【语音】【视频】等类型的信息（注意是这几种类型，不是这几个文字），查看不同格式的回复。\r\nSDK官方地址：https://sdk.weixin.senparc.com\r\n"",
      ""ToUserName"": ""FromUserName(OpenId)"",
      ""FromUserName"": ""ToUserName"",
      ""CreateTime"": ""2019-09-22T23:13:24.8832361+08:00""
    },
    {
      ""MsgType"": 0,
      ""Content"": ""您刚才发送了文字信息：天气\r\n\r\n您刚才还发送了如下消息（10/0）：\r\n23:13:24 【Text】天气\r\n23:13:22 【Text】天气\r\n23:13:21 【Text】天气\r\n23:13:20 【Text】天气\r\n23:11:13 【Text】天气\r\n23:11:05 【Text】12345\r\n23:08:32 【Text】苏州\r\n23:07:06 【Text】天气\r\n23:04:51 【Text】111222\r\n\r\n\r\n如果您在3分钟内连续发送消息，记录将被自动保留（当前设置：最多记录10条）。过期后记录将会自动清除。\r\n\r\n\r\n您还可以发送【位置】【图片】【语音】【视频】等类型的信息（注意是这几种类型，不是这几个文字），查看不同格式的回复。\r\nSDK官方地址：https://sdk.weixin.senparc.com\r\n"",
      ""ToUserName"": ""FromUserName(OpenId)"",
      ""FromUserName"": ""ToUserName"",
      ""CreateTime"": ""2019-09-22T23:14:06.8772982+08:00""
    },
    {
      ""MsgType"": 0,
      ""Content"": ""您刚才发送了文字信息：天气\r\n\r\n您刚才还发送了如下消息（10/0）：\r\n23:14:06 【Text】天气\r\n23:13:24 【Text】天气\r\n23:13:22 【Text】天气\r\n23:13:21 【Text】天气\r\n23:13:20 【Text】天气\r\n23:11:13 【Text】天气\r\n23:11:05 【Text】12345\r\n23:08:32 【Text】苏州\r\n23:07:06 【Text】天气\r\n\r\n\r\n如果您在3分钟内连续发送消息，记录将被自动保留（当前设置：最多记录10条）。过期后记录将会自动清除。\r\n\r\n\r\n您还可以发送【位置】【图片】【语音】【视频】等类型的信息（注意是这几种类型，不是这几个文字），查看不同格式的回复。\r\nSDK官方地址：https://sdk.weixin.senparc.com\r\n"",
      ""ToUserName"": ""FromUserName(OpenId)"",
      ""FromUserName"": ""ToUserName"",
      ""CreateTime"": ""2019-09-22T23:14:08.1252882+08:00""
    },
    {
      ""MsgType"": 0,
      ""Content"": ""您刚才发送了文字信息：天气\r\n\r\n您刚才还发送了如下消息（10/0）：\r\n23:14:08 【Text】天气\r\n23:14:06 【Text】天气\r\n23:13:24 【Text】天气\r\n23:13:22 【Text】天气\r\n23:13:21 【Text】天气\r\n23:13:20 【Text】天气\r\n23:11:13 【Text】天气\r\n23:11:05 【Text】12345\r\n23:08:32 【Text】苏州\r\n\r\n\r\n如果您在3分钟内连续发送消息，记录将被自动保留（当前设置：最多记录10条）。过期后记录将会自动清除。\r\n\r\n\r\n您还可以发送【位置】【图片】【语音】【视频】等类型的信息（注意是这几种类型，不是这几个文字），查看不同格式的回复。\r\nSDK官方地址：https://sdk.weixin.senparc.com\r\n"",
      ""ToUserName"": ""FromUserName(OpenId)"",
      ""FromUserName"": ""ToUserName"",
      ""CreateTime"": ""2019-09-22T23:14:08.7938944+08:00""
    },
    {
      ""MsgType"": 0,
      ""Content"": ""您刚才发送了文字信息：天气\r\n\r\n您刚才还发送了如下消息（10/0）：\r\n23:14:08 【Text】天气\r\n23:14:08 【Text】天气\r\n23:14:06 【Text】天气\r\n23:13:24 【Text】天气\r\n23:13:22 【Text】天气\r\n23:13:21 【Text】天气\r\n23:13:20 【Text】天气\r\n23:11:13 【Text】天气\r\n23:11:05 【Text】12345\r\n\r\n\r\n如果您在3分钟内连续发送消息，记录将被自动保留（当前设置：最多记录10条）。过期后记录将会自动清除。\r\n\r\n\r\n您还可以发送【位置】【图片】【语音】【视频】等类型的信息（注意是这几种类型，不是这几个文字），查看不同格式的回复。\r\nSDK官方地址：https://sdk.weixin.senparc.com\r\n"",
      ""ToUserName"": ""FromUserName(OpenId)"",
      ""FromUserName"": ""ToUserName"",
      ""CreateTime"": ""2019-09-22T23:14:09.414496+08:00""
    },
    {
      ""MsgType"": 0,
      ""Content"": ""您刚才发送了文字信息：天气\r\n\r\n您刚才还发送了如下消息（10/1）：\r\n23:14:08 【Text】天气\r\n23:14:08 【Text】天气\r\n23:14:08 【Text】天气\r\n23:14:06 【Text】天气\r\n23:13:24 【Text】天气\r\n23:13:22 【Text】天气\r\n23:13:21 【Text】天气\r\n23:13:20 【Text】天气\r\n23:11:13 【Text】天气\r\n\r\n\r\n如果您在3分钟内连续发送消息，记录将被自动保留（当前设置：最多记录10条）。过期后记录将会自动清除。\r\n\r\n\r\n您还可以发送【位置】【图片】【语音】【视频】等类型的信息（注意是这几种类型，不是这几个文字），查看不同格式的回复。\r\nSDK官方地址：https://sdk.weixin.senparc.com\r\n"",
      ""ToUserName"": ""FromUserName(OpenId)"",
      ""FromUserName"": ""ToUserName"",
      ""CreateTime"": ""2019-09-22T23:14:09.9626979+08:00""
    },
    {
      ""MsgType"": 0,
      ""Content"": ""您刚才发送了文字信息：天气\r\n\r\n您刚才还发送了如下消息（10/0）：\r\n23:14:08 【Text】天气\r\n23:14:08 【Text】天气\r\n23:14:08 【Text】天气\r\n23:14:08 【Text】天气\r\n23:14:06 【Text】天气\r\n23:13:24 【Text】天气\r\n23:13:22 【Text】天气\r\n23:13:21 【Text】天气\r\n23:13:20 【Text】天气\r\n\r\n\r\n如果您在3分钟内连续发送消息，记录将被自动保留（当前设置：最多记录10条）。过期后记录将会自动清除。\r\n\r\n\r\n您还可以发送【位置】【图片】【语音】【视频】等类型的信息（注意是这几种类型，不是这几个文字），查看不同格式的回复。\r\nSDK官方地址：https://sdk.weixin.senparc.com\r\n"",
      ""ToUserName"": ""FromUserName(OpenId)"",
      ""FromUserName"": ""ToUserName"",
      ""CreateTime"": ""2019-09-22T23:14:10.2830586+08:00""
    },
    {
      ""MsgType"": 0,
      ""Content"": ""您刚才发送了文字信息：1234\r\n\r\n您刚才还发送了如下消息（10/0）：\r\n23:14:10 【Text】天气\r\n23:14:08 【Text】天气\r\n23:14:08 【Text】天气\r\n23:14:08 【Text】天气\r\n23:14:08 【Text】天气\r\n23:14:06 【Text】天气\r\n23:13:24 【Text】天气\r\n23:13:22 【Text】天气\r\n23:13:21 【Text】天气\r\n\r\n\r\n如果您在3分钟内连续发送消息，记录将被自动保留（当前设置：最多记录10条）。过期后记录将会自动清除。\r\n\r\n\r\n您还可以发送【位置】【图片】【语音】【视频】等类型的信息（注意是这几种类型，不是这几个文字），查看不同格式的回复。\r\nSDK官方地址：https://sdk.weixin.senparc.com\r\n"",
      ""ToUserName"": ""FromUserName(OpenId)"",
      ""FromUserName"": ""ToUserName"",
      ""CreateTime"": ""2019-09-22T23:17:50.6362189+08:00""
    },
    {
      ""MsgType"": 0,
      ""Content"": ""您刚才发送了文字信息：1234\r\n\r\n您刚才还发送了如下消息（10/0）：\r\n23:17:49 【Text】1234\r\n23:14:10 【Text】天气\r\n23:14:08 【Text】天气\r\n23:14:08 【Text】天气\r\n23:14:08 【Text】天气\r\n23:14:08 【Text】天气\r\n23:14:06 【Text】天气\r\n23:13:24 【Text】天气\r\n23:13:22 【Text】天气\r\n\r\n\r\n如果您在3分钟内连续发送消息，记录将被自动保留（当前设置：最多记录10条）。过期后记录将会自动清除。\r\n\r\n\r\n您还可以发送【位置】【图片】【语音】【视频】等类型的信息（注意是这几种类型，不是这几个文字），查看不同格式的回复。\r\nSDK官方地址：https://sdk.weixin.senparc.com\r\n"",
      ""ToUserName"": ""FromUserName(OpenId)"",
      ""FromUserName"": ""ToUserName"",
      ""CreateTime"": ""2019-09-22T23:17:53.8077989+08:00""
    },
    {
      ""MsgType"": 0,
      ""Content"": ""您刚才发送了文字信息：1234\r\n\r\n您刚才还发送了如下消息（10/0）：\r\n23:17:53 【Text】1234\r\n23:17:49 【Text】1234\r\n23:14:10 【Text】天气\r\n23:14:08 【Text】天气\r\n23:14:08 【Text】天气\r\n23:14:08 【Text】天气\r\n23:14:08 【Text】天气\r\n23:14:06 【Text】天气\r\n23:13:24 【Text】天气\r\n\r\n\r\n如果您在3分钟内连续发送消息，记录将被自动保留（当前设置：最多记录10条）。过期后记录将会自动清除。\r\n\r\n\r\n您还可以发送【位置】【图片】【语音】【视频】等类型的信息（注意是这几种类型，不是这几个文字），查看不同格式的回复。\r\nSDK官方地址：https://sdk.weixin.senparc.com\r\n"",
      ""ToUserName"": ""FromUserName(OpenId)"",
      ""FromUserName"": ""ToUserName"",
      ""CreateTime"": ""2019-09-22T23:17:54.3355697+08:00""
    }
  ],
  ""MaxRecordCount"": 10,
  ""StorageDataTypeName"": ""System.Int32"",
  ""StorageData"": 0,
  ""ExpireMinutes"": null,
  ""AppStoreState"": 2,
  ""CurrentAppDataItem"": {
    ""Name"": ""查天气"",
    ""Id"": ""3"",
    ""Note"": null,
    ""Version"": ""0.1.0"",
    ""ExpireTime"": 1571832729,
    ""ExpireDateTime"": ""2019-10-23T20:12:09+08:00"",
    ""MessageEnterWord"": ""天气"",
    ""MessageExitWord"": ""退出"",
    ""MessageKeywords"": null,
    ""MessageKeepTime"": 1
  }
}";

            var dt1 = SystemTime.Now;
            var jsonResult = JsonConvert.DeserializeObject<CustomMessageContext>(jsonStr, new MessageContextJsonConverter<CustomMessageContext, RequestMessageBase, ResponseMessageBase>());
            Console.WriteLine(SystemTime.NowDiff(dt1).TotalMilliseconds + "ms");
            Assert.IsNotNull(jsonResult);
            Console.WriteLine(jsonResult);
        }
    }
}
