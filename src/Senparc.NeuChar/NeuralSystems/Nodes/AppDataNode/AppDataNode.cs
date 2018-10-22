using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.NeuralSystems
{
    /// <summary>
    /// 订阅 APP 的信息
    /// </summary>
    public class AppDataNode : BaseNeuralNode
    {
        public int NeuralAppId { get; set; }

        /// <summary>
        /// 设置
        /// </summary>
        new public AppDataConfigs Config { get; set; } = new AppDataConfigs();
    }
}
