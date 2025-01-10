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
    
    文件名：RequestMessageBase.cs
    文件功能描述：接收请求消息基类
    
    
    创建标识：Senparc - 20150211
    
    修改标识：Senparc - 20150303
    修改描述：整理接口
----------------------------------------------------------------*/



namespace Senparc.NeuChar.Entities
{
    /// <summary>
    /// 请求消息基础接口
    /// </summary>
    public interface IRequestMessageBase : IMessageBase
    {
        //删除MsgType因为企业号和公众号的MsgType为两个独立的枚举类型
        //RequestMessageType MsgType { get; }
        long MsgId { get; set; }

        RequestMsgType MsgType { get; set; }
        string Encrypt { get; set; }

        /// <summary>
        /// 除重业务代码，默认不填即可 通常设置企业微信事件 通常为事件类型
        /// 使用场景：企业微信除重redis key 通常以企业分隔 从而锁key和事件缓存key 会把整个企业微信事件存入redis。所以新增业务编码减少储存 提升读写速度和区分锁 避免整个企业获取同一个锁key
        /// </summary>
        string RepeatedBusiness { get; set; }

        /// <summary>
        /// 默认空 如果没有则依然返回空 不影响原有，如果有值则返回横杠加业务，如-business
        /// </summary>
        string GetRepeatedBusiness { get; }
    }

    /// <summary>
    /// 接收到请求的消息基类
    /// </summary>
    public class RequestMessageBase : MessageBase, IRequestMessageBase
    {
        public RequestMessageBase()
        {

        }

        //public virtual RequestMessageType MsgType
        //{
        //    get { return RequestMessageType.Text; }
        //}

        public long MsgId { get; set; }
        public virtual RequestMsgType MsgType { get; set; }
        public string Encrypt { get; set; }

        /// <summary>
        /// 除重业务代码，默认不填即可 通常设置企业微信事件
        /// 使用场景：企业微信除重redis key 通常以企业分隔 从而锁key和事件缓存key 会把整个企业微信事件存入redis。所以新增业务编码减少储存 提升读写速度和区分锁 避免整个企业获取同一个锁key
        /// </summary>
        public virtual string RepeatedBusiness { get; set; }

        public string GetRepeatedBusiness
        {
            get
            {
                return string.IsNullOrEmpty(RepeatedBusiness) ? "" : $"-{RepeatedBusiness}";
            }
        }
    }
}
