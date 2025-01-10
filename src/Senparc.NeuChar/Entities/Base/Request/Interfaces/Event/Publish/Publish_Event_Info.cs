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
    
    文件名：Publish_Event_Info.cs
    文件功能描述：Publish_Event_Info
    
    
    创建标识：Senparc - 20230628

----------------------------------------------------------------*/

using System.Collections.Generic;

namespace Senparc.NeuChar.Entities
{
    public class Publish_Event_Info
    {
        /// <summary>
        /// 发布任务id
        /// </summary>
        public string publish_id { get; set; }

        /// <summary>
        /// 发布状态
        /// 0:成功, 1:发布中，2:原创失败, 3: 常规失败, 4:平台审核不通过, 5:成功后用户删除所有文章, 6: 成功后系统封禁所有文章
        /// </summary>

        public int publish_status { get; set; }

        /// <summary>
        /// 当发布状态为0时（即成功）时，返回图文的 article_id，可用于“客服消息”场景
        /// </summary>

        public string article_id { get; set; }

        /// <summary>
        /// 文章详情
        /// </summary>
        public Article_Detail article_detail { get; set; }

        /// <summary>
        /// 当发布状态为2或4时，返回不通过的文章编号，第一篇为 1；其他发布状态则为空
        /// </summary>
        public List<int> fail_idx { get; set; }
    }
}
