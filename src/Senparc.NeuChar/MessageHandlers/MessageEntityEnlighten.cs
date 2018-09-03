using Senparc.NeuChar.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.MessageHandlers
{
    /// <summary>
    /// MessageHandler初始化请求和响应消息的定义类
    /// </summary>
    public class MessageEntityEnlighten
    {
        #region 不同消息类型明确示例类型的委托

        #region 请求消息

        public Func<IRequestMessageText> NewRequestMessageText { get; }
        public Func<IRequestMessageImage> NewRequestMessageImage { get; }

        #endregion

        #region 响应消息

        public Func<IResponseMessageText> NewResponseMessageText { get; }
        public Func<IResponseMessageNews> NewResponseMessageNews { get; }
        public Func<IResponseMessageMusic> NewResponseMessageMusic { get; }
        public Func<IResponseMessageImage> NewResponseMessageImage { get; }
        public Func<IResponseMessageVoice> NewResponseMessageVoice { get; }
        public Func<IResponseMessageVideo> NewResponseMessageVideo { get; }
        public Func<IResponseMessageTransfer_Customer_Service> NewResponseMessageTransfer_Customer_Service { get; }

        #endregion

        #region 生成

        #endregion

        #endregion
    }
}
