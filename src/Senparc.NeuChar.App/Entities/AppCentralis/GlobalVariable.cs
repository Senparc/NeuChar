using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senparc.NeuChar.App.Entities.AppCentralis
{
    /// <summary>
    /// 单个应用中枢中的全局变量
    /// </summary>
    [Serializable]
    public class GlobalVariable
    {
        /// <summary>
        /// 变量名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 变量值
        /// </summary>
        public string Value { get; set; }
    }
}
