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
    
    文件名：NeuCharAppPostModel.cs
    文件功能描述：定义枚举文件
    
    
    创建标识：Senparc - 20191003
    
    修改标识：Senparc - 20191003
    修改描述：从 PostModel 改名为 NeuCharAppPostModel

----------------------------------------------------------------*/

namespace Senparc.NeuChar.App.Entities
{
    /// <summary>
    /// 给 NeuCharAppController 使用的 PostModel
    /// </summary>
    public class NeuCharAppPostModel : EncryptPostModel
    {
        //TODO:类名重命名
        public override string DomainId { get => AppId; set => AppId = value; }

        /// <summary>
        /// 当前请求对应的 NeuChar App 的 AppId
        /// </summary>
        public string AppId { get; set; }
    }
}
