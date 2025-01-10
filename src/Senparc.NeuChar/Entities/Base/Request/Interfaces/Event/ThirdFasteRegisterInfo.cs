#region Apache License Version 2.0
/*----------------------------------------------------------------

Copyright 2025 Suzhou Senparc Network Technology Co.,Ltd.

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
    Copyright (C) 2025 Senparc
    
    文件名：ThirdFasteRegisterInfo.cs
    文件功能描述：第三方快速注册小程序的注册审核事件推送中的info
    
    
    创建标识：Senparc - 20190529
----------------------------------------------------------------*/

namespace Senparc.NeuChar.Entities
{
    /// <summary>
    /// 第三方快速注册小程序的注册审核事件推送中的info
    /// </summary>
    public class ThirdFasteRegisterInfo
    {
        #region 企业
        /// <summary>
        /// 企业小程序 - 企业名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 企业小程序 - 企业代码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 企业小程序 - 企业代码类型 
        /// </summary>
        public int code_type { get; set; }
        /// <summary>
        /// 企业小程序 - 法人微信号
        /// </summary>
        public string legal_persona_wechat { get; set; }
        /// <summary>
        /// 企业小程序 - 法人姓名
        /// </summary>
        public string legal_persona_name { get; set; }
        #endregion

        #region 公共
        /// <summary>
        /// 公共 - 第三方联系电话
        /// </summary>
        public string component_phone { get; set; }
        #endregion

        #region 个人 
        /// <summary>
        /// 个人小程序 - 用户微信号
        /// </summary>
        public string wxuser { get; set; }
        /// <summary>
        /// 个人小程序 - 用户姓名
        /// </summary>
        public string idname { get; set; }
        #endregion
    }
}
