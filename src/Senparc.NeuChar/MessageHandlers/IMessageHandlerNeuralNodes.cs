using Senparc.NeuChar.NeuralSystems;

namespace Senparc.NeuChar.MessageHandlers
{
    /// <summary>
    /// 用于提供MessageHandler中的 NeuralSystem 的设置节点
    /// </summary>
    public interface IMessageHandlerNeuralNodes
    {
        /// <summary>
        /// 请求和响应消息有差别化的定义
        /// </summary>
        MessageHandlerNode CurrentMessageHandlerNode { get; }

        /// <summary>
        /// 请求和响应消息有差别化的定义
        /// </summary>
        AppDataNode CurrentAppDataNode { get; }
    }
}
