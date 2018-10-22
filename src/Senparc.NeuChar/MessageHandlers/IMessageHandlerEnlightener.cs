using Senparc.NeuChar.ApiHandlers;

namespace Senparc.NeuChar.MessageHandlers
{
    /// <summary>
    /// 用于提供MessageHandler中的“请求及响应”详细类型定义
    /// </summary>
    public interface IMessageHandlerEnlightener
    {

        /// <summary>
        /// 请求和响应消息有差别化的定义
        /// </summary>
        MessageEntityEnlightener MessageEntityEnlightener { get; }

        /// <summary>
        /// 请求和响应消息有差别化的定义
        /// </summary>
        ApiEnlightener ApiEnlightener { get; }
    }
}
