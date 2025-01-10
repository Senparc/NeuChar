using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.Entities
{
    /// <summary>
    /// 所有RequestMessageText的接口
    /// </summary>
    public interface IRequestMessageText: IRequestMessageBase
    {
        /// <summary>
        /// 文本消息内容
        /// </summary>
        string Content { get; set; }
    }
}
