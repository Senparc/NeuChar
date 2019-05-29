#region Apache License Version 2.0
/*----------------------------------------------------------------

Copyright 2018 Suzhou Senparc Network Technology Co.,Ltd.

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
    Copyright (C) 2018 Senparc
    
    文件名：SendLocationInfo.cs
    文件功能描述：弹出地理位置选择器的事件推送中的SendLocationInfo
    
    
    创建标识：Senparc - 20150211
    
    修改标识：Senparc - 20150303
    修改描述：整理接口
----------------------------------------------------------------*/

namespace Senparc.NeuChar.Entities
{
    /// <summary>
    /// 弹出地理位置选择器的事件推送中的SendLocationInfo
    /// </summary>
    public class ThirdFasteRegisterInfo
    {
        /// <summary>
        /// 企业名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 企业代码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 企业代码类型 
        /// </summary>
        public int code_type { get; set; }
        /// <summary>
        /// 法人微信号
        /// </summary>
        public string legal_persona_wechat { get; set; }
        /// <summary>
        /// 法人姓名
        /// </summary>
        public string legal_persona_name { get; set; }
        /// <summary>
        /// 第三方联系电话
        /// </summary>
        public string component_phone { get; set; }
    }
}
