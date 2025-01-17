﻿#pragma warning disable 1591

namespace Senparc.NeuChar.NeuralSystems
{
    /// <summary>
    /// 用于传入设置的基础模型
    /// </summary>
    public class ConfigRoot
    {
        public string Version { get; set; }
        public string Name { get; set; }
        public string Guid { get; set; }
        public string Key { get; set; }

        /// <summary>
        /// 详细的配置信息
        /// </summary>
        public object[] Configs { get; set; }
        //public List<BaseNeuralNode> Configs { get; set; }
    }

}
