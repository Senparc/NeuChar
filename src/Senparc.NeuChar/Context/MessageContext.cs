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
    
    文件名：MessageContext.cs
    文件功能描述：微信消息上下文（单个用户）接口
    
    
    创建标识：Senparc - 20150211
    
    修改标识：Senparc - 20150303
    修改描述：整理接口
    
    修改标识：Senparc - 20150708
    修改描述：完善备注
    
    修改标识：Senparc - 20181226
    修改描述：v0.5.2 修改 DateTime 为 DateTimeOffset

    修改标识：Senparc - 20190914
    修改描述：v0.8.0 
              1、提供支持分布式缓存的消息上下文（MessageContext）
              2、将 IMessageContext<TRequest, TResponse> 接口中 TRequest、TResponse 约束为 class
              3、IMessageContext 接口添加 GetRequestEntityMappingResult() 和 GetResponseEntityMappingResult() 方法

----------------------------------------------------------------*/

/* 注意：修改此文件的借口和属性是，需要同步修改 MessageContextJsonConverter 中的赋值，否则可能导致上下文读取时属性值缺失 */

using System;
using System.Xml.Linq;
using Newtonsoft.Json;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.NeuralSystems;

namespace Senparc.NeuChar.Context
{
    /// <summary>
    /// 微信消息上下文（单个用户）接口
    /// </summary>
    /// <typeparam name="TRequest">请求消息类型</typeparam>
    /// <typeparam name="TResponse">响应消息类型</typeparam>
    public interface IMessageContext<TRequest, TResponse>
        where TRequest : class, IRequestMessageBase
        where TResponse : class, IResponseMessageBase
    {
        /// <summary>
        /// 用户名（OpenID）
        /// </summary>
        string UserName { get; set; }
        /// <summary>
        /// 最后一次活动时间（用户主动发送Resquest请求的时间）
        /// </summary>
        DateTimeOffset? LastActiveTime { get; set; }
        /// <summary>
        /// 本次活动时间（当前消息收到的时间）
        /// </summary>
        DateTimeOffset? ThisActiveTime { get; set; }
        /// <summary>
        /// 接收消息记录
        /// </summary>
        MessageContainer<TRequest> RequestMessages { get; set; }
        /// <summary>
        /// 响应消息记录
        /// </summary>
        MessageContainer<TResponse> ResponseMessages { get; set; }
        /// <summary>
        /// 最大储存容量（分别针对RequestMessages和ResponseMessages）
        /// </summary>
        int MaxRecordCount { get; set; }

        /// <summary>
        /// StorageData的类型名称
        /// </summary>
        string StorageDataTypeName { get; set; }
        /// <summary>
        /// StorageData的类型
        /// </summary>
        Type StorageDataType { get; set; }
        /// <summary>
        /// 临时储存数据，如用户状态等，出于保持.net 3.5版本，这里暂不使用dynamic
        /// </summary>
        object StorageData { get; set; }

        /// <summary>
        /// 用于覆盖WeixinContext所设置的默认过期时间
        /// </summary>
        Double? ExpireMinutes { get; set; }

        /// <summary>
        /// AppStore状态，系统属性，请勿操作
        /// </summary>
        AppStoreState AppStoreState { get; set; }

        /// <summary>
        /// 当前正在服务的 AppDataItem
        /// </summary>
        AppDataItem CurrentAppDataItem { get; set; }

        event EventHandler<WeixinContextRemovedEventArgs<TRequest, TResponse>> MessageContextRemoved;

        void OnRemoved();

        /// <summary>
        /// 从 Xml 转换 RequestMessage 对象的处理（只是创建实例，不填充数据） 
        /// </summary>
        /// <param name="requestMsgType">RequestMsgType</param>
        /// <param name="doc">RequestMessage 的明文 XML</param>
        /// <returns></returns>
        TRequest GetRequestEntityMappingResult(RequestMsgType requestMsgType, XDocument doc);

        /// <summary>
        /// 从 Xml 转换 RequestMessage 对象的处理（只是创建实例，不填充数据） 
        /// </summary>
        /// <param name="responseMsgType">RequestMsgType</param>
        /// <param name="doc">ResponseMessage 的明文 XML</param>
        /// <returns></returns>
        TResponse GetResponseEntityMappingResult(ResponseMsgType responseMsgType, XDocument doc);
    }

    /// <summary>
    /// 微信消息上下文（单个用户）
    /// </summary>
    public abstract class MessageContext<TRequest, TResponse> : IMessageContext<TRequest, TResponse>
        where TRequest : class, IRequestMessageBase
        where TResponse : class, IResponseMessageBase
    {
        private int _maxRecordCount;

        /// <summary>
        /// 用户识别ID（微信中为 OpenId）
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 最后一次活动时间（用户主动发送Resquest请求的时间）
        /// </summary>
        public DateTimeOffset? LastActiveTime { get; set; }
        /// <summary>
        /// 本次活动时间（当前消息收到的时间）
        /// </summary>
        public DateTimeOffset? ThisActiveTime { get; set; }

        //[JsonConverter(typeof(MessageContextJsonConverter))]
        public MessageContainer<TRequest> RequestMessages { get; set; }

        //[JsonConverter(typeof(MessageContextJsonConverter))]
        public MessageContainer<TResponse> ResponseMessages { get; set; }

        /// <summary>
        /// 最大允许记录数
        /// </summary>
        public int MaxRecordCount
        {
            get
            {
                return _maxRecordCount;
            }
            set
            {
                //消息列表中调整最大记录数
                RequestMessages.MaxRecordCount = value;
                ResponseMessages.MaxRecordCount = value;

                _maxRecordCount = value;
            }
        }

        /// <summary>
        /// StorageData的类型名称
        /// </summary>
        public string StorageDataTypeName { get; set; }

        /// <summary>
        /// StorageData的类型
        /// </summary>
        [JsonIgnore]
        public Type StorageDataType { get; set; }

        private object _storageData = null;
        public object StorageData
        {
            get
            {
                return _storageData;
            }
            set
            {
                _storageData = value;
                if (value != null)
                {
                    StorageDataTypeName = value.GetType().FullName;
                }
                else
                {
                    StorageDataTypeName = null;
                }
            }
        }

        public Double? ExpireMinutes { get; set; }

        /// <summary>
        /// AppStore状态，系统属性，请勿操作
        /// </summary>
        public AppStoreState AppStoreState { get; set; }

        /// <summary>
        /// 当前正在服务的 AppDataItem
        /// </summary>
        public AppDataItem CurrentAppDataItem { get; set; }


        /// <summary>
        /// 当MessageContext被删除时触发的事件
        /// </summary>
        public virtual event EventHandler<WeixinContextRemovedEventArgs<TRequest, TResponse>> MessageContextRemoved = null;

        /// <summary>
        /// 执行上下文被移除的事件
        /// 注意：此事件不是实时触发的，而是等过期后任意一个人发过来的下一条消息执行之前触发。
        /// </summary>
        /// <param name="e"></param>
        private void OnMessageContextRemoved(WeixinContextRemovedEventArgs<TRequest, TResponse> e)
        {
            EventHandler<WeixinContextRemovedEventArgs<TRequest, TResponse>> temp = MessageContextRemoved;

            if (temp != null)
            {
                temp(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ///// <param name="maxRecordCount">maxRecordCount如果小于等于0，则不限制</param>
        public MessageContext(/*MessageContainer<IRequestMessageBase> requestMessageContainer,
            MessageContainer<IResponseMessageBase> responseMessageContainer*/)
        {
            /*
             * 注意：即使使用其他类实现IMessageContext，
             * 也务必在这里进行下面的初始化，尤其是设置当前时间，
             * 这个时间关系到及时从缓存中移除过期的消息，节约内存使用
             */

            RequestMessages = new MessageContainer<TRequest>(MaxRecordCount);
            ResponseMessages = new MessageContainer<TResponse>(MaxRecordCount);
            LastActiveTime = SystemTime.Now;
        }

        /// <summary>
        /// 从 Xml 转换 RequestMessage 对象的处理（只是创建实例，不填充数据） 
        /// </summary>
        /// <param name="requestMsgType">RequestMsgType</param>
        /// <param name="doc">RequestMessage 的明文 XML</param>
        /// <returns></returns>
        public abstract TRequest GetRequestEntityMappingResult(RequestMsgType requestMsgType, XDocument doc);

        /// <summary>
        /// 从 Xml 转换 RequestMessage 对象的处理（只是创建实例，不填充数据） 
        /// </summary>
        /// <param name="responseMsgType">RequestMsgType</param>
        /// <param name="doc">ResponseMessage 的明文 XML</param>
        /// <returns></returns>
        public abstract TResponse GetResponseEntityMappingResult(ResponseMsgType responseMsgType, XDocument doc);

        /// <summary>
        /// 此上下文被清除的时候触发
        /// </summary>
        public virtual void OnRemoved()
        {
            var onRemovedArg = new WeixinContextRemovedEventArgs<TRequest, TResponse>(this);
            OnMessageContextRemoved(onRemovedArg);
        }
    }
}
