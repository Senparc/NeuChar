using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar
{
    /// <summary>
    /// 自动绑定属性
    /// </summary>
    public class ApiBindAttributes
    {
        /// <summary>
        /// 平台类型
        /// </summary>
        public PlatformType PlatformType { get; set; } = PlatformType.General;

        /// <summary>
        /// 平台内唯一名称（如使用 PlatformType.General，请使用宇宙唯一名称）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// ApiBindAttributes 构造函数
        /// </summary>
        public ApiBindAttributes() { }

        /// <summary>
        /// ApiBindAttributes 构造函数
        /// </summary>
        /// <param name="platformType">平台类型</param>
        /// <param name="name">平台内唯一名称（如使用 PlatformType.General，请使用宇宙唯一名称）</param>
        public ApiBindAttributes(PlatformType platformType, string name)
        {
            PlatformType = platformType;
            Name = name;
        }
    }
}
