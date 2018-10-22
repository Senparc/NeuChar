using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.NeuralSystems
{
    /// <summary>
    /// 配置项约束接口
    /// </summary>
    public interface IConfigItem
    {
        /// <summary>
        /// 备注名称
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// 唯一编号
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        string Note { get; set; }
        /// <summary>
        /// 版本号，如："2018.9.27.1"
        /// </summary>
        string Version { get; set; }
    }
}
