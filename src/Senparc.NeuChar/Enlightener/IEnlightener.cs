using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.Enlightener
{
    /// <summary>
    /// 所有 Enlightener 定义的接口
    /// </summary>
    public interface IEnlightener
    {
        /// <summary>
        /// 支持平台类型
        /// </summary>
        PlatformType PlatformType { get; set; }
    }
}
