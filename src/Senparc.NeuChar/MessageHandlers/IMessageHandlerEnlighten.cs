using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.MessageHandlers
{
    /// <summary>
    /// 用于提供MessageHandler中的“请求及响应”详细类型定义
    /// </summary>
    public interface IMessageHandlerEnlighten
    {
        MessageEntityEnlighten MessageEntityEnlighten { get; }

    }
}
