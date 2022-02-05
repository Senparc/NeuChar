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
    Copyright (C) 2022 Senparc
    
    文件名：SubscribeMsgPopupEvent.cs
    文件功能描述：用户操作订阅通知弹窗
    
    
    创建标识：ccccccmd - 20210305
----------------------------------------------------------------*/


using System.Collections.Generic;

namespace Senparc.NeuChar.Entities
{


    /// <summary>
    /// 用户操作订阅通知弹窗
    /// </summary>
    public class SubscribeMsgPopupEvent
    {

        /// <summary>
        /// 模板 id 
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// 	用户点击行为（同意、取消发送通知）
        /// </summary>
        public string SubscribeStatusString { get; set; }

        /// <summary>
        /// 场景 1:弹窗来自 H5 页面      2:弹窗来自图文消息
        /// </summary>
        public int PopupScene { get; set; }
    }
}