using Microsoft.VisualStudio.TestTools.UnitTesting;
using Senparc.NeuChar.Helpers;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Senparc.NeuChar.Entities;

namespace Senparc.NeuChar.Tests.Helpers
{
    [TestClass]
    public class EntityHelperTests
    {
        /// <summary>
        /// 公众号第三方平台推送消息类型
        /// </summary>
        public enum RequestInfoType
        {
            /// <summary>
            /// 推送component_verify_ticket协议
            /// </summary>
            component_verify_ticket,

            /// <summary>
            /// 推送取消授权通知
            /// </summary>
            unauthorized,

            /// <summary>
            /// 更新授权
            /// </summary>
            updateauthorized,

            /// <summary>
            /// 授权成功通知
            /// </summary>
            authorized
        }

        public class RequestMessageComponentVerifyTicket
        {
            public RequestInfoType InfoType => RequestInfoType.component_verify_ticket;
            public string AppId { get; set; }
            public DateTimeOffset CreateTime { get; set; }

            public string ComponentVerifyTicket { get; set; }
        }

        public class RequestMessageEvent_MassSendJobFinish
        {
            public string Event { get; set; }

            /// <summary>
            /// 群发的结构，为“send success”或“send fail”或“err(num)”。当send success时，也有可能因用户拒收公众号的消息、系统错误等原因造成少量用户接收失败。err(num)是审核失败的具体原因，可能的情况如下：
            /// err(10001), //涉嫌广告 err(20001), //涉嫌政治 err(20004), //涉嫌社会 err(20002), //涉嫌色情 err(20006), //涉嫌违法犯罪 err(20008), //涉嫌欺诈 err(20013), //涉嫌版权 err(22000), //涉嫌互推(互相宣传) err(21000), //涉嫌其他
            /// </summary>
            public string Status { get; set; }

            /// <summary>
            /// group_id下粉丝数；或者openid_list中的粉丝数
            /// </summary>
            public int TotalCount { get; set; }

            /// <summary>
            /// 过滤（过滤是指，有些用户在微信设置不接收该公众号的消息）后，准备发送的粉丝数，原则上，FilterCount = SentCount + ErrorCount
            /// </summary>
            public int FilterCount { get; set; }

            /// <summary>
            /// 发送成功的粉丝数
            /// </summary>
            public int SentCount { get; set; }

            /// <summary>
            /// 发送失败的粉丝数
            /// </summary>
            public int ErrorCount { get; set; }

            /// <summary>
            /// 群发的消息ID
            /// </summary>
            public long MsgID { get; set; }

            [Obsolete("请使用MsgID")]
            public new long MsgId { get; set; }

            /// <summary>
            /// CopyrightCheckResult
            /// </summary>
            public CopyrightCheckResult CopyrightCheckResult { get; set; }

            /// <summary>
            /// 群发文章的url
            /// </summary>
            public ArticleUrlResult ArticleUrlResult { get; set; }

            public RequestMessageEvent_MassSendJobFinish()
            {
                CopyrightCheckResult = new CopyrightCheckResult();
                ArticleUrlResult = new ArticleUrlResult();
            }
        }


        [TestMethod]
        public void FillEntityWithXmlTest()
        {
            string xml = @"<xml>
  <AppId><![CDATA[wxbbd3f07e2945cf2a]]></AppId>
  <CreateTime>1536250240</CreateTime>
  <InfoType><![CDATA[component_verify_ticket]]></InfoType>
  <ComponentVerifyTicket><![CDATA[ticket@@@oj3_KoMMnlH2SZbBs89CAzABbNF3Wfr12AFbIg19aKeLSgebFJXPE9tS60X38ab-kG5a08oskSWVdHEklNCAhA]]></ComponentVerifyTicket>
</xml>";
            RequestMessageComponentVerifyTicket entity = new RequestMessageComponentVerifyTicket();
            XDocument doc = XDocument.Parse(xml);
            EntityHelper.FillEntityWithXml(entity, doc);

            Assert.AreEqual("wxbbd3f07e2945cf2a", entity.AppId);
            //Assert.AreEqual("CreateTime", entity.AppId);
            Assert.AreEqual(RequestInfoType.component_verify_ticket, entity.InfoType); //只读
            Assert.AreEqual(
                "ticket@@@oj3_KoMMnlH2SZbBs89CAzABbNF3Wfr12AFbIg19aKeLSgebFJXPE9tS60X38ab-kG5a08oskSWVdHEklNCAhA",
                entity.ComponentVerifyTicket);

            try
            {
                entity = null; //测试 null 的情况
                EntityHelper.FillEntityWithXml(entity, doc);
                Assert.Fail();
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine("抛出异常是正确的");
            }
        }

        [TestMethod]
        public void FillRequestMessageEvent_MassSendJobFinishWithXmlTest()
        {
            var xml = @"<xml>
<ToUserName><![CDATA[gh_a96a4a619366]]></ToUserName>
<FromUserName><![CDATA[oR5Gjjl_eiZoUpGozMo7dbBJ362A]]></FromUserName>
<CreateTime>1394524295</CreateTime>
<MsgType><![CDATA[event]]></MsgType>
<Event><![CDATA[MASSSENDJOBFINISH]]></Event>
<MsgID>1988</MsgID>
<Status><![CDATA[sendsuccess]]></Status>
<TotalCount>100</TotalCount>
<FilterCount>80</FilterCount>
<SentCount>75</SentCount>
<ErrorCount>5</ErrorCount>
<CopyrightCheckResult> 
    <Count>2</Count>  
    <ResultList> 
      <item> 
        <ArticleIdx>1</ArticleIdx>  
        <UserDeclareState>0</UserDeclareState>  
        <AuditState>2</AuditState>  
        <OriginalArticleUrl><![CDATA[Url_1]]></OriginalArticleUrl>  
        <OriginalArticleType>1</OriginalArticleType>  
        <CanReprint>1</CanReprint>  
        <NeedReplaceContent>1</NeedReplaceContent>  
        <NeedShowReprintSource>1</NeedShowReprintSource> 
      </item>  
      <item> 
        <ArticleIdx>2</ArticleIdx>  
        <UserDeclareState>0</UserDeclareState>  
        <AuditState>2</AuditState>  
        <OriginalArticleUrl><![CDATA[Url_2]]></OriginalArticleUrl>  
        <OriginalArticleType>1</OriginalArticleType>  
        <CanReprint>1</CanReprint>  
        <NeedReplaceContent>1</NeedReplaceContent>  
        <NeedShowReprintSource>1</NeedShowReprintSource> 
      </item> 
    </ResultList>  
    <CheckState>2</CheckState> 
  </CopyrightCheckResult> 
  <ArticleUrlResult>
    <Count>1</Count>
    <ResultList>
      <item>
        <ArticleIdx>1</ArticleIdx>
        <ArticleUrl><![CDATA[http://mp.weixin.qq.com/s/sssss]]></ArticleUrl>
      </item>
    </ResultList>
  </ArticleUrlResult>
</xml>";

            var entity = new RequestMessageEvent_MassSendJobFinish();
            XDocument doc = XDocument.Parse(xml);

            EntityHelper.FillEntityWithXml(entity, doc);



            Assert.IsTrue(entity.CopyrightCheckResult.Count == 2);

            Assert.IsTrue(entity.CopyrightCheckResult.ResultList[1].item.OriginalArticleUrl == "Url_2");


            Assert.IsTrue(entity.ArticleUrlResult.Count == 1);

            Assert.IsTrue(entity.ArticleUrlResult.ResultList[0].item.ArticleUrl == "http://mp.weixin.qq.com/s/sssss");
        }


        public class RequestMessageEvent_SubscribeMsgPopupEvent : RequestMessageBase
        {
            public List<SubscribeMsgPopupEvent> SubscribeMsgPopupEvent { get; set; }
        }


        [TestMethod]
        public void FillRequestMessageEvent_SubscribeMsgPopupEventhWithXmlTest()
        {
            var xml = @"<xml>
    <ToUserName><![CDATA[gh_123456789abc]]></ToUserName>
    <FromUserName><![CDATA[otFpruAK8D-E6EfStSYonYSBZ8_4]]></FromUserName>
    <CreateTime>1610969440</CreateTime>
    <MsgType><![CDATA[event]]></MsgType>
    <Event><![CDATA[subscribe_msg_popup_event]]></Event>
    <SubscribeMsgPopupEvent>
        <List>
            <TemplateId><![CDATA[VRR0UEO9VJOLs0MHlU0OilqX6MVFDwH3_3gz3Oc0NIc]]></TemplateId>
            <SubscribeStatusString><![CDATA[accept]]></SubscribeStatusString>
            <PopupScene>2</PopupScene>
        </List>
        <List>
            <TemplateId><![CDATA[9nLIlbOQZC5Y89AZteFEux3WCXRRRG5Wfzkpssu4bLI]]></TemplateId>
            <SubscribeStatusString><![CDATA[reject]]></SubscribeStatusString>
            <PopupScene>2</PopupScene>
        </List>
    </SubscribeMsgPopupEvent>
</xml>";

            var entity = new RequestMessageEvent_SubscribeMsgPopupEvent();

            XDocument doc = XDocument.Parse(xml);


            EntityHelper.FillEntityWithXml(entity, doc);


            Assert.IsTrue(entity.SubscribeMsgPopupEvent.Count == 2);

            Assert.IsTrue(entity.SubscribeMsgPopupEvent[1].TemplateId ==
                          "9nLIlbOQZC5Y89AZteFEux3WCXRRRG5Wfzkpssu4bLI");

            Assert.IsTrue(entity.SubscribeMsgPopupEvent[0].PopupScene == 2);
        }


        public class RequestMessageEvent_SubscribeMsgChangeEvent : RequestMessageBase
        {
            public List<SubscribeMsgChangeEvent> SubscribeMsgChangeEvent { get; set; }
        }


        [TestMethod]
        public void FillRequestMessageEvent_SubscribeMsgChangeEventWithXmlTest()
        {
            var xml = @"<xml>
    <ToUserName><![CDATA[gh_123456789abc]]></ToUserName>
    <FromUserName><![CDATA[otFpruAK8D-E6EfStSYonYSBZ8_4]]></FromUserName>
    <CreateTime>1610969440</CreateTime>
    <MsgType><![CDATA[event]]></MsgType>
    <Event><![CDATA[subscribe_msg_change_event]]></Event>
    <SubscribeMsgChangeEvent>
        <List>
            <TemplateId><![CDATA[VRR0UEO9VJOLs0MHlU0OilqX6MVFDwH3_3gz3Oc0NIc]]></TemplateId>
            <SubscribeStatusString><![CDATA[reject]]></SubscribeStatusString>
        </List>
         <List>
            <TemplateId><![CDATA[xxxxxxxxxxxx]]></TemplateId>
            <SubscribeStatusString><![CDATA[reject]]></SubscribeStatusString>
        </List>
    </SubscribeMsgChangeEvent>
</xml>";

            var entity = new RequestMessageEvent_SubscribeMsgChangeEvent();

            XDocument doc = XDocument.Parse(xml);


            EntityHelper.FillEntityWithXml(entity, doc);


            Assert.IsTrue(entity.SubscribeMsgChangeEvent.Count == 2);

            Assert.IsTrue(entity.SubscribeMsgChangeEvent[0].TemplateId ==
                          "VRR0UEO9VJOLs0MHlU0OilqX6MVFDwH3_3gz3Oc0NIc");

            Assert.IsTrue(entity.SubscribeMsgChangeEvent[1].SubscribeStatusString == "reject");
        }



        public class RequestMessageEvent_SubscribeMsgSentEvent : RequestMessageBase
        {
            public List<SubscribeMsgSentEvent> SubscribeMsgSentEvent { get; set; }
        }


        [TestMethod]
        public void FillRequestMessageEvent_SubscribeMsgSentEventWithXmlTest()
        {
            var xml = @"<xml>
    <ToUserName><![CDATA[gh_123456789abc]]></ToUserName>
    <FromUserName><![CDATA[otFpruAK8D-E6EfStSYonYSBZ8_4]]></FromUserName>
    <CreateTime>1610969468</CreateTime>
    <MsgType><![CDATA[event]]></MsgType>
    <Event><![CDATA[subscribe_msg_sent_event]]></Event>
    <SubscribeMsgSentEvent>
        <List>
            <TemplateId><![CDATA[VRR0UEO9VJOLs0MHlU0OilqX6MVFDwH3_3gz3Oc0NIc]]></TemplateId>
            <MsgID>1700827132819554304</MsgID>
            <ErrorCode>0</ErrorCode>
            <ErrorStatus><![CDATA[success]]></ErrorStatus>
        </List>
         <List>
            <TemplateId><![CDATA[xxxxxxxxxxxxxdddddd2131f3d1fd]]></TemplateId>
            <MsgID>17008271328xxx19554304</MsgID>
            <ErrorCode>0</ErrorCode>
            <ErrorStatus><![CDATA[success]]></ErrorStatus>
        </List>
    </SubscribeMsgSentEvent>
</xml>";

            var entity = new RequestMessageEvent_SubscribeMsgSentEvent();

            XDocument doc = XDocument.Parse(xml);


            EntityHelper.FillEntityWithXml(entity, doc);


            Assert.IsTrue(entity.SubscribeMsgSentEvent.Count == 2);

            Assert.IsTrue(entity.SubscribeMsgSentEvent[1].TemplateId ==
                          "xxxxxxxxxxxxxdddddd2131f3d1fd");

            Assert.IsTrue(entity.SubscribeMsgSentEvent[0].MsgID == "1700827132819554304");
        }

        #region FillUnknowNodeTest

        [TestMethod]
        public void FillUnknowNodeTest()
        {
            var xml = @"
<xml>
  <ToUserName><![CDATA[wx7618c0a6d9358622]]></ToUserName>
  <FromUserName><![CDATA[sys]]></FromUserName>
  <CreateTime>1644320363</CreateTime>
  <MsgType><![CDATA[event]]></MsgType>
  <Event><![CDATA[sys_approval_change]]></Event>
  <AgentID>3010040</AgentID>
  <ApprovalInfo>
    <SpNo>202202080001</SpNo>
    <Applyer>
      <UserId><![CDATA[001]]></UserId>
      <Party><![CDATA[1]]></Party>
    </Applyer>
  </ApprovalInfo>
</xml>
";
            var entity = new RequestMessageEvent_Unkown();

            XDocument doc = XDocument.Parse(xml);

            EntityHelper.FillEntityWithXml(entity, doc);

            Assert.IsNotNull(entity.ApprovalInfo);
            Assert.AreEqual(202202080001, entity.ApprovalInfo.SpNo);
            Assert.IsNotNull(entity.ApprovalInfo.Applyer);
            Assert.AreEqual("001", entity.ApprovalInfo.Applyer.UserId);
        }

        public class RequestMessageEvent_Unkown : RequestMessageBase
        {
            public ApprovalInfo ApprovalInfo { get; set; }
        }

        public class ApprovalInfo
        {
            public long SpNo { get; set; }
            public Applyer Applyer { get; set; }
        }
        public class Applyer
        {
            public string UserId { get; set; }
            public string Party { get; set; }
        }

        #endregion


    }
}