using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.Enlighten
{
    /// <summary>
    /// 所有 Enlighten 定义的接口
    /// </summary>
    public interface IEnlighten
    {
        /// <summary>
        /// 支持平台类型
        /// </summary>
        PlatformType PlatformType { get; set; }
    }
}
