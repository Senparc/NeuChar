#region Apache License Version 2.0
/*----------------------------------------------------------------

Copyright 2021 Suzhou Senparc Network Technology Co.,Ltd.

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.

Detail: https://github.com/JeffreySu/WeiXinMPSDK/blob/master/license.md

----------------------------------------------------------------*/
#endregion Apache License Version 2.0

/*----------------------------------------------------------------
    Copyright (C) 2021 Senparc
    
    文件名：ApiBindAttribute.cs
    文件功能描述：微信等平台使用的自动绑定属性
    
    
    创建标识：Senparc - 20180914

    修改标识：Senparc - 20210705
    修改描述：v1.5 重构到 CO2NET 的 WebApiEngine

----------------------------------------------------------------*/

using System;

namespace Senparc.NeuChar
{
    /// <summary>
    /// 自动绑定属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class NcApiBindAttribute : Senparc.CO2NET.ApiBindAttribute
    {
        ///// <summary>
        ///// 平台类型
        ///// </summary>
        //public PlatformType PlatformType { get; set; } = PlatformType.General;

        ///// <summary>
        ///// 平台内唯一名称（如使用 PlatformType.General，请使用宇宙唯一名称）
        ///// </summary>
        //public string Name { get; set; }

        /// <summary>
        /// 是否需要使用 AccessToken
        /// </summary>
        public bool NeedAccessToken { get; set; }

        /// <summary>
        /// ApiBindAttributes 构造函数
        /// </summary>
        public NcApiBindAttribute() { }

        /// <summary>
        /// ApiBindAttributes 构造函数
        /// </summary>
        /// <param name="platformType">平台类型</param>
        /// <param name="name">平台内唯一名称（如使用 PlatformType.General，请使用宇宙唯一名称）</param>
        /// <param name="needAccessToken">是否需要使用 AccessToken</param>
        public NcApiBindAttribute(PlatformType platformType, string name, bool needAccessToken)
            : base(platformType.ToString(), name)
        {
            //PlatformType = platformType;
            //Name = name;
            NeedAccessToken = needAccessToken;
        }
    }
}
