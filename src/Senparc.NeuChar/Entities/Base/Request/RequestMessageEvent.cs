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
    
    文件名：RequestMessageEvent.cs
    文件功能描述：所有事件消息的基类
    
    TODO：此类暂时没有用到，预留方便今后扩展时重构
    
    创建标识：Senparc - 20181006
    
----------------------------------------------------------------*/



namespace Senparc.NeuChar.Entities
{
    /// <summary>
    /// 所有事件消息的基类
    /// </summary>
    public abstract class RequestMessageEvent : RequestMessageBase, IRequestMessageEvent
    {
        /// <summary>
        /// 事件类型
        /// </summary>
        public abstract object EventType { get; }
        /// <summary>
        /// 获取事件类型的字符串
        /// </summary>
        public virtual string EventName
        {
            get
            {
                return EventType?.ToString();
            }
        }

        /// <summary>
        /// RequestMessageEvent 构造函数
        /// </summary>
        public RequestMessageEvent()
        {

        }

    }
}
