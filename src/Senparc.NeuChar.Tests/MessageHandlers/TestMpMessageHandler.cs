using Senparc.NeuChar.Context;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Helpers;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.MessageContexts;
using Senparc.Weixin.MP.MessageHandlers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Senparc.NeuChar.Tests.MessageHandlers
{
    /// <summary>
    /// 测试公众号 MessageHandler
    /// </summary>
    public class TestMpMessageHandler : MessageHandler<DefaultMpMessageContext> /*MessageHandler<TestMpMessageContext>*/
    {
        public TestMpMessageHandler(XDocument requestDoc, PostModel postModel = null, int maxRecordCount = 0)
                : base(requestDoc, postModel, maxRecordCount)
        {
        }

        public TestMpMessageHandler(RequestMessageBase requestMessage, PostModel postModel = null, int maxRecordCount = 0)
            : base(requestMessage, postModel, maxRecordCount)
        {
        }

        public override async Task<IResponseMessageBase> OnTextRequestAsync(RequestMessageText requestMessage)
        {
            var response = base.CreateResponseMessage<ResponseMessageText>();
            response.Content = requestMessage.Content;
            return response;
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            var responseMessage = this.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "来自单元测试消息的默认响应消息";
            return responseMessage;
        }
    }
}
