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
    
    文件名：MessageHandler.cs
    文件功能描述：微信请求的集中处理方法
    
    
    创建标识：Senparc - 20150211
    
    修改标识：Senparc - 20150303
    修改描述：整理接口

    修改标识：Senparc - 20160909
    修改描述：v4.7.8 修正在ResponseMessage都null的情况下，
              没有对_textResponseMessage做判断就直接返回空字符串的问题

    修改标识：Senparc - 20170409
    修改描述：v4.11.8 （MessageHandler V3.2）修复 TextResponseMessage 不输出加密信息的问题

    修改标识：Senparc - 20170409
    修改描述：v4.12.4  MessageHandler基类默认开启消息去重

    -- NeuChar --

    修改标识：Senparc - 20181118
    修改描述：v0.4.3 

----------------------------------------------------------------*/


/*
 * V3.2
 * V4.0 添加异步方法
 */

using System;
using System.IO;
using System.Xml.Linq;
using Senparc.CO2NET.Utilities;
using Senparc.NeuChar.ApiHandlers;
using Senparc.NeuChar.Context;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Helpers;
using Senparc.NeuChar.NeuralSystems;
using Senparc.NeuChar.Exceptions;
using Senparc.CO2NET.APM;

namespace Senparc.NeuChar.MessageHandlers
{
    /// <summary>
    /// 微信请求的集中处理方法
    /// 此方法中所有过程，都基于Senparc.NeuChar.基础功能，只为简化代码而设。
    /// </summary>
    public abstract partial class MessageHandler<TC, TRequest, TResponse> : IMessageHandlerWithContext<TC, TRequest, TResponse>
        where TC : class, IMessageContext<TRequest, TResponse>, new()
        where TRequest : IRequestMessageBase
        where TResponse : IResponseMessageBase
    {

        #region 上下文

        ///// <summary>
        ///// 上下文
        ///// </summary>
        //public static WeixinContext<TC> GlobalWeixinContext = new WeixinContext<TC>();

        /// <summary>
        /// 全局消息上下文
        /// </summary>
        [Obsolete("请使用 GlobalMessageContext")]
        public GlobalMessageContext<TC, TRequest, TResponse> WeixinContext { get { return GlobalMessageContext; } }

        /// <summary>
        /// 全局消息上下文
        /// </summary>
        public abstract GlobalMessageContext<TC, TRequest, TResponse> GlobalMessageContext { get; }


        /// <summary>
        /// 当前用户消息上下文
        /// </summary>
        public virtual TC CurrentMessageContext { get { return GlobalMessageContext.GetMessageContext(RequestMessage); } }

        /// <summary>
        /// 忽略重复发送的同一条消息（通常因为微信服务器没有收到及时的响应）
        /// </summary>
        public bool OmitRepeatedMessage { get; set; }

        /// <summary>
        /// 消息是否已经被去重
        /// </summary>
        public bool MessageIsRepeated { get; set; }


        #endregion

        /// <summary>
        /// 请求和响应消息有差别化的定义
        /// </summary>
        public abstract MessageEntityEnlightener MessageEntityEnlightener { get; }

        /// <summary>
        /// 请求和响应消息有差别化的定义
        /// </summary>
        public abstract ApiEnlightener ApiEnlightener { get; }

        private MessageHandlerNode _currentMessageHandlerNode;
        /// <summary>
        /// 默认 MessageHandlerNode 对象
        /// </summary>
        public MessageHandlerNode CurrentMessageHandlerNode
        {
            get
            {
                if (_currentMessageHandlerNode == null)
                {
                    //TODO:Neuchar：在这里先做一次NeuChar标准的判断

                    var neuralSystem = NeuralSystem.Instance;

                    //获取当前设置节点
                    _currentMessageHandlerNode = (neuralSystem.GetNode("MessageHandlerNode") as MessageHandlerNode) ?? new MessageHandlerNode();
                }
                return _currentMessageHandlerNode;
            }
            set
            {
                _currentMessageHandlerNode = value;
            }
        }

        private AppDataNode _currentAppDataNode;
        /// <summary>
        /// 当前 App 订阅信息
        /// </summary>
        public AppDataNode CurrentAppDataNode
        {
            get
            {
                if (_currentAppDataNode == null)
                {
                    //TODO:Neuchar：在这里先做一次NeuChar标准的判断

                    var neuralSystem = NeuralSystem.Instance;
                    //获取当前设置节点
                    _currentAppDataNode = (neuralSystem.GetNode("AppDataNode") as AppDataNode) ?? new AppDataNode();
                }
                return _currentAppDataNode;  
            }
            set
            {
                _currentAppDataNode = value;
            }
        }


        /// <summary>
        /// 发送者用户名（OpenId）
        /// </summary>
        public string OpenId { get { return RequestMessage != null ? RequestMessage.FromUserName : null; } }

        /// <summary>
        /// 发送者用户名（OpenId）
        /// </summary>
        [Obsolete("请使用OpenId")]
        public string WeixinOpenId { get { return OpenId; } }

        /// <summary>
        /// 
        /// </summary>
        [Obsolete("UserName属性从v0.6起已过期，建议使用WeixinOpenId")]
        public string UserName { get { return OpenId; } }

        /// <summary>
        /// 取消执行Execute()方法。一般在OnExecuting()中用于临时阻止执行Execute()。
        /// 默认为False。
        /// 如果在执行OnExecuting()执行前设为True，则所有OnExecuting()、Execute()、OnExecuted()代码都不会被执行。
        /// 如果在执行OnExecuting()执行过程中设为True，则后续Execute()及OnExecuted()代码不会被执行。
        /// 建议在设为True的时候，给ResponseMessage赋值，以返回友好信息。
        /// </summary>
        public bool CancelExcute { get; set; }

        /// <summary>
        /// 在构造函数中转换得到原始XML数据
        /// </summary>
        public XDocument RequestDocument { get; set; }

        /// <summary>
        /// 根据ResponseMessageBase获得转换后的ResponseDocument
        /// 注意：这里每次请求都会根据当前的ResponseMessageBase生成一次，如需重用此数据，建议使用缓存或局部变量
        /// </summary>
        public abstract XDocument ResponseDocument { get; }

        /// <summary>
        /// 最后返回的ResponseDocument。
        /// 如果是Senparc.NeuChar.QY，则应当和ResponseDocument一致；如果是Senparc.NeuChar.QY，则应当在ResponseDocument基础上进行加密
        /// </summary>
        public abstract XDocument FinalResponseDocument { get; }

        //protected Stream InputStream { get; set; }
        /// <summary>
        /// 请求实体
        /// </summary>
        public virtual TRequest RequestMessage { get; set; }
        /// <summary>
        /// 响应实体
        /// 正常情况下只有当执行Execute()方法后才可能有值。
        /// 也可以结合Cancel，提前给ResponseMessage赋值。
        /// </summary>
        public virtual TResponse ResponseMessage { get; set; }

        /// <summary>
        /// 是否使用了MessageAgent代理
        /// </summary>
        public bool UsedMessageAgent { get; set; }

        /// <summary>
        /// 是否使用了加密消息格式
        /// </summary>
        public bool UsingEcryptMessage { get; set; }


        /// <summary>
        /// 原始的加密请求（如果不加密则为null）
        /// </summary>
        public XDocument EcryptRequestDocument { get; set; }

        /// <summary>
        /// 是否使用了兼容模式加密信息
        /// </summary>
        public bool UsingCompatibilityModelEcryptMessage { get; set; }


        private string _textResponseMessage = null;

        /// <summary>
        /// 文字类型返回消息
        /// </summary>
        public string TextResponseMessage
        {
            get
            {
                if (ResponseMessage != null && ResponseMessage is SuccessResponseMessageBase)
                {
                    _textResponseMessage = (ResponseMessage as SuccessResponseMessageBase).ReturnText;//返回"success"
                }

                if (_textResponseMessage == null //原先为 _textResponseMessage != null     ——Jeffrey Su 2017.06.01
                    && (ResponseMessage == null || ResponseMessage is IResponseMessageNoResponse))
                {
                    return "";//返回空消息
                }

                if (_textResponseMessage == null)
                {
                    return /*ResponseDocument == null ? null : */
                            FinalResponseDocument != null
                            ? FinalResponseDocument.ToString()
                            : "";
                    //ResponseDocument.ToString();
                }
                else
                {
                    return _textResponseMessage;
                }
            }
            set
            {
                _textResponseMessage = value;
            }
        }

        public IEncryptPostModel PostModel { get; set; }

        protected DateTimeOffset ExecuteStatTime { get; set; }

        /// <summary>
        /// 构造函数公用的初始化方法
        /// </summary>
        /// <param name="postDataDocument"></param>
        /// <param name="maxRecordCount"></param>
        /// <param name="postModel"></param>
        public void CommonInitialize(XDocument postDataDocument, int maxRecordCount, IEncryptPostModel postModel)
        {
            OmitRepeatedMessage = true;//默认开启去重
            GlobalMessageContext.MaxRecordCount = maxRecordCount;
            PostModel = postModel;//PostModel 在当前类初始化过程中必须赋值
            RequestDocument = Init(postDataDocument, postModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="maxRecordCount"></param>
        /// <param name="postModel">需要传入到Init的参数</param>
        public MessageHandler(Stream inputStream, IEncryptPostModel postModel, int maxRecordCount = 0)
        {
            var postDataDocument = XmlUtility.Convert(inputStream);
            //PostModel = postModel;//PostModel 在当前类初始化过程中必须赋值
            CommonInitialize(postDataDocument, maxRecordCount, postModel);
        }

        /// <summary>
        /// 使用postDataDocument的构造函数
        /// </summary>
        /// <param name="postDataDocument"></param>
        /// <param name="maxRecordCount"></param>
        /// <param name="postModel">需要传入到Init的参数</param>
        public MessageHandler(XDocument postDataDocument, IEncryptPostModel postModel, int maxRecordCount = 0)
        {
            //PostModel = postModel;//PostModel 在当前类初始化过程中必须赋值
            CommonInitialize(postDataDocument, maxRecordCount, postModel);
        }

        /// <summary>
        /// <para>使用requestMessageBase的构造函数</para>
        /// <para>此构造函数提供给具体的类库进行测试使用，例如Senparc.NeuChar.Work</para>
        /// </summary>
        /// <param name="requestMessageBase"></param>
        /// <param name="maxRecordCount"></param>
        /// <param name="postModel">需要传入到Init的参数</param>
        public MessageHandler(RequestMessageBase requestMessageBase, IEncryptPostModel postModel, int maxRecordCount = 0)
        {
            ////将requestMessageBase生成XML格式。
            //var xmlStr = XmlUtility.XmlUtility.Serializer(requestMessageBase);
            //var postDataDocument = XDocument.Parse(xmlStr);

            //CommonInitialize(postDataDocument, maxRecordCount, postData);

            //此方法不执行任何方法，提供给具体的类库进行测试使用，例如Senparc.NeuChar.Work

            PostModel = postModel;//PostModel 在当前类初始化过程中必须赋值
        }


        /// <summary>
        /// 初始化，获取RequestDocument。
        /// Init中需要对上下文添加当前消息（如果使用上下文）；以及判断消息的加密情况，对解密信息进行解密
        /// </summary>
        /// <param name="requestDocument"></param>
        /// <param name="postModel"></param>
        public abstract XDocument Init(XDocument requestDocument, IEncryptPostModel postModel);

        //public abstract TR CreateResponseMessage<TR>() where TR : ResponseMessageBase;

        /// <summary>
        /// 根据当前的 RequestMessage 创建指定类型（RT）的 ResponseMessage
        /// </summary>
        /// <typeparam name="TR"></typeparam>
        /// <returns></returns>
        public virtual TR CreateResponseMessage<TR>() where TR : ResponseMessageBase
        {
            if (RequestMessage == null)
            {
                return null;
            }

            return RequestMessage.CreateResponseMessage<TR>(this.MessageEntityEnlightener);
        }

        /// <summary>
        /// 在 Execute() 之前运行，可以使用 CancelExcute=true 中断后续 Execute() 和 OnExecuted() 方法的执行
        /// </summary>
        public virtual void OnExecuting()
        {
        }

        /// <summary>
        /// 执行微信请求（如果没有被 CancelExcute=true 中断）
        /// </summary>
        public void Execute()
        {
            //进行 APM 记录
            ExecuteStatTime = SystemTime.Now;

            DataOperation apm = new DataOperation(PostModel?.DomainId);
            apm.Set(NeuCharApmKind.Message_Request.ToString(), 1, tempStorage: OpenId);

            if (CancelExcute)
            {
                return;
            }

            OnExecuting();

            if (CancelExcute)
            {
                return;
            }

            try
            {
                if (RequestMessage == null)
                {
                    return;
                }

                BuildResponseMessage();

                //记录上下文
                //此处修改
                if (MessageContextGlobalConfig.UseMessageContext && ResponseMessage != null && !string.IsNullOrEmpty(ResponseMessage.FromUserName))
                {
                    GlobalMessageContext.InsertMessage(ResponseMessage);
                }
                apm.Set(NeuCharApmKind.Message_SuccessResponse.ToString(), 1, tempStorage: OpenId);
            }
            catch (Exception ex)
            {
                apm.Set(NeuCharApmKind.Message_Exception.ToString(), 1, tempStorage: OpenId);
                throw new MessageHandlerException("MessageHandler中Execute()过程发生错误：" + ex.Message, ex);
            }
            finally
            {
                OnExecuted();
                apm.Set(NeuCharApmKind.Message_ResponseMillisecond.ToString(), (SystemTime.Now - this.ExecuteStatTime).TotalMilliseconds, tempStorage: OpenId);
            }
        }

        public abstract void BuildResponseMessage();

        /// <summary>
        /// 在 Execute() 之后运行（如果没有被 CancelExcute=true 中断）
        /// </summary>
        public virtual void OnExecuted()
        {
        }



        ///// <summary>
        ///// 默认返回消息（当任何OnXX消息没有被重写，都将自动返回此默认消息）
        ///// </summary>
        //public abstract IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage);

    }
}
