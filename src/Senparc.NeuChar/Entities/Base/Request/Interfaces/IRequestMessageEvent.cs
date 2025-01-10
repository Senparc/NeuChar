using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.Entities
{
    /// <summary>
    /// 所有RequestMessageShortEvent的接口
    /// </summary>
    public interface IRequestMessageEvent : IRequestMessageBase
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        object EventType { get; }
        /// <summary>
        /// 获取事件类型的字符串
        /// </summary>
        string EventName { get; }
    }
}
