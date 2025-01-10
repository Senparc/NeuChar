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
    
    文件名：Article_Detail.cs
    文件功能描述：文章详情
    
    
    创建标识：Senparc - 20230628

----------------------------------------------------------------*/

using System.Collections.Generic;

namespace Senparc.NeuChar.Entities
{
    /// <summary>
    /// 文章详情
    /// </summary>
    public class Article_Detail
    {
        /// <summary>
        /// 当发布状态为0时（即成功）时，返回文章数量
        /// </summary>
        public int count { get; set; }

        /// <summary>
        /// 文章详情项
        /// </summary>
        public List<Article_Detail_Item> item { get; set; }
    }

    /// <summary>
    /// 文章详情项
    /// </summary>
    public class Article_Detail_Item
    {
        /// <summary>
        /// 当发布状态为0时（即成功）时，返回文章对应的编号
        /// </summary>
        public int idx { get; set; }
        /// <summary>
        /// 当发布状态为0时（即成功）时，返回图文的永久链接
        /// </summary>
        public string article_url { get; set; }
    }
}
