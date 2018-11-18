using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.MessageHandlers
{
    /// <summary>
    /// NeuCharActionType.CheckNeuChar 下的 ConfigRoot 类型
    /// </summary>
    public class APMDomainConfig
    {
        /// <summary>
        /// Domain 名称，默认为 AppId
        /// </summary>
        public string Domain { get; set; }
        /// <summary>
        /// 是否读取后移除缓存信息（默认为 true）
        /// </summary>
        public bool RemoveData { get; set; } = true;
    }
}
