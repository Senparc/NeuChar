using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar
{
    /// <summary>
    /// 自动绑定属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class ApiBindAttribute : Attribute
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
        /// 是否需要使用 AccessToken
        /// </summary>
        public bool NeedAccessToken { get; set; }

        /// <summary>
        /// ApiBindAttributes 构造函数
        /// </summary>
        public ApiBindAttribute() { }

        /// <summary>
        /// ApiBindAttributes 构造函数
        /// </summary>
        /// <param name="platformType">平台类型</param>
        /// <param name="name">平台内唯一名称（如使用 PlatformType.General，请使用宇宙唯一名称）</param>
        /// <param name="needAccessToken">是否需要使用 AccessToken</param>
        public ApiBindAttribute(PlatformType platformType, string name, bool needAccessToken)
        {
            PlatformType = platformType;
            Name = name;
            NeedAccessToken = needAccessToken;
        }
    }
}
