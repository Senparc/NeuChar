using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.NeuralSystems
{
    /// <summary>
    /// APP 订阅设置（包含所有 APP 订阅）
    /// </summary>
    public class AppDataConfigs : NeuralNodeConfig
    {
        /// <summary>
        /// APP 订阅详情
        /// </summary>
        public List<AppDataItem> AppDataItems { get; set; } = new List<AppDataItem>();
    }
}
