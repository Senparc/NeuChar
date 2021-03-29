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
    
    文件名：SubscribeMsgSentEvent.cs
    文件功能描述：发送订阅通知
    
    
    创建标识：ccccccmd - 20210305
----------------------------------------------------------------*/


using System.Collections.Generic;

namespace Senparc.NeuChar.Entities
{


    /// <summary>
    /// 发送订阅通知
    /// </summary>
    public class SubscribeMsgSentEvent
    {


        /// <summary>
        /// 模板 id 
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// 推送结果状态码（0表示成功）
        /// </summary>
        public int ErrorCode { get; set; }

        /// <summary>
        /// 推送结果状态码文字含义
        /// </summary>
        public string ErrorStatus { get; set; }

        /// <summary>
        /// 消息 id
        /// </summary>
        public string MsgID { get; set; }


    }
}