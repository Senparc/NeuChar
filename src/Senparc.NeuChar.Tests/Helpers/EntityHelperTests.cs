using Microsoft.VisualStudio.TestTools.UnitTesting;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

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
            public  RequestInfoType InfoType
            {
                get { return RequestInfoType.component_verify_ticket; }
            }
            public string AppId { get; set; }
            public DateTimeOffset CreateTime { get; set; }

            public string ComponentVerifyTicket { get; set; }
        }


        [TestMethod]
        public void FillEntityWithXml()
        {
            var xml = @"<xml>
  <AppId><![CDATA[wxbbd3f07e2945cf2a]]></AppId>
  <CreateTime>1536250240</CreateTime>
  <InfoType><![CDATA[component_verify_ticket]]></InfoType>
  <ComponentVerifyTicket><![CDATA[ticket@@@oj3_KoMMnlH2SZbBs89CAzABbNF3Wfr12AFbIg19aKeLSgebFJXPE9tS60X38ab-kG5a08oskSWVdHEklNCAhA]]></ComponentVerifyTicket>
</xml>";
            var entity = new RequestMessageComponentVerifyTicket();
            var doc = XDocument.Parse(xml);
            EntityHelper.FillEntityWithXml(entity, doc);

            Assert.AreEqual("wxbbd3f07e2945cf2a", entity.AppId);
            //Assert.AreEqual("CreateTime", entity.AppId);
            Assert.AreEqual(RequestInfoType.component_verify_ticket, entity.InfoType);//只读
            Assert.AreEqual("ticket@@@oj3_KoMMnlH2SZbBs89CAzABbNF3Wfr12AFbIg19aKeLSgebFJXPE9tS60X38ab-kG5a08oskSWVdHEklNCAhA", entity.ComponentVerifyTicket);

        }
    }
}
