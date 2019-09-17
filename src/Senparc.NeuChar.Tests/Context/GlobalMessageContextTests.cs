using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Senparc.CO2NET.Extensions;
using Senparc.NeuChar.Context;
using Senparc.NeuChar.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Senparc.NeuChar.Tests.Context
{
    [TestClass]
    public class GlobalMessageContextTests
    {
        #region 测试数据
        string jsonStr = @"{
  ""UserName"": ""FromUserName(OpenId)"",
  ""LastActiveTime"": ""2019-09-18T08:20:33.6845115+00:00"",
  ""ThisActiveTime"": ""2019-09-17T16:20:36.150931+00:00"",
  ""RequestMessages"": [
    {
      ""MsgType"": 0,
      ""Content"": ""111"",
      ""bizmsgmenuid"": """",
      ""MsgId"": 637043340269519607,
      ""Encrypt"": """",
      ""ToUserName"": ""ToUserName"",
      ""FromUserName"": ""FromUserName(OpenId)"",
      ""CreateTime"": ""2019-09-17T16:20:32+08:00""
    },
    {
      ""MsgType"": 0,
      ""Content"": ""111222"",
      ""bizmsgmenuid"": """",
      ""MsgId"": 637043340269529600,
      ""Encrypt"": """",
      ""ToUserName"": ""ToUserName"",
      ""FromUserName"": ""FromUserName(OpenId)"",
      ""CreateTime"": ""2019-09-17T16:20:35+08:00""
    }
  ],
  ""ResponseMessages"": [
    {
      ""MsgType"": 0,
      ""Content"": ""您刚才发送了文字信息：111\r\n\r\n如果您在3分钟内连续发送消息，记录将被自动保留（当前设置：最多记录10条）。过期后记录将会自动清除。\r\n\r\n\r\n您还可以发送【位置】【图片】【语音】【视频】等类型的信息（注意是这几种类型，不是这几个文字），查看不同格式的回复。\r\nSDK官方地址：https://sdk.weixin.senparc.com\r\n"",
      ""ToUserName"": ""FromUserName(OpenId)"",
      ""FromUserName"": ""ToUserName"",
      ""CreateTime"": ""2019-09-17T16:20:34.0151428+08:00""
    },
    {
      ""MsgType"": 0,
      ""Content"": ""您刚才发送了文字信息：111222\r\n\r\n如果您在3分钟内连续发送消息，记录将被自动保留（当前设置：最多记录10条）。过期后记录将会自动清除。\r\n\r\n\r\n您还可以发送【位置】【图片】【语音】【视频】等类型的信息（注意是这几种类型，不是这几个文字），查看不同格式的回复。\r\nSDK官方地址：https://sdk.weixin.senparc.com\r\n"",
      ""ToUserName"": ""FromUserName(OpenId)"",
      ""FromUserName"": ""ToUserName"",
      ""CreateTime"": ""2019-09-17T16:20:36.1982099+08:00""
    }
  ],
  ""MaxRecordCount"": 10,
  ""StorageData"": null,
  ""ExpireMinutes"": null,
  ""AppStoreState"": 0,
  ""CurrentAppDataItem"": null
}";
        #endregion
        [TestMethod]
        public void ConvertTest()
        {
            var jsonResult = JsonConvert.DeserializeObject<CustomMessageContext>(jsonStr, new MessageContextJsonConverter<CustomMessageContext, RequestMessageBase, ResponseMessageBase>());
            Assert.IsNotNull(jsonResult);
            Console.WriteLine(jsonResult.ToJson());
            Assert.AreEqual("FromUserName(OpenId)", jsonResult.UserName);
        }

    }
}
