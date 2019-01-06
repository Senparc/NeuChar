using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.Entities.App
{
    /// <summary>
    /// 拉取请求的数据
    /// </summary>
    public class PushConfigRequestData
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// NeuChar App Id
        /// </summary>
        public int AppId { get; set; }

        /// <summary>
        /// 配置信息
        /// </summary>
        public ConfigParamModel Config { get; set; }
    }
}
