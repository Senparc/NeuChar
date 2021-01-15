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
    
    文件名：WeixinContext.cs
    文件功能描述：微信消息上下文（全局）
    
    
    创建标识：Senparc - 20150211
    
    修改标识：Senparc - 20150303
    修改描述：整理接口
    
    修改标识：Senparc - 20181023
    修改描述：修改 timeSpan 获取判断逻辑（firstMessageContext.LastActiveTime 已改为 DateTime? 类型）

    修改标识：Senparc - 20190914
    修改描述：（V3.0）v0.8.0 提供支持分布式缓存的消息上下文（MessageContext）

----------------------------------------------------------------*/

/*
 * V3.2
 * V4.0 添加异步方法
 * v5.0 支持分布式缓存
 */


#pragma warning disable 1591
using Newtonsoft.Json;
using Senparc.CO2NET.Cache;
using Senparc.NeuChar.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Senparc.NeuChar.Context
{
    /// <summary>
    /// 消息上下文全局设置
    /// </summary>
    public static class MessageContextGlobalConfig//TODO:所有设置可以整合到一起
    {
        ///// <summary>
        ///// 上下文操作使用的同步锁
        ///// </summary>
        public const string MESSAGE_CONTENT_ITEM_LOCK_NAME = "MESSAGE_CONTENT_ITEM_LOCK_NAME";

        ///// <summary>
        ///// 去重专用锁
        ///// </summary>
        public const string MESSAGE_INSERT_LOCK_NAME = "MESSAGE_INSERT_LOCK_NAME";


        /// <summary>
        /// 缓存键前缀
        /// </summary>
        public const string CACHE_KEY_PREFIX = "MessageContext:";

        /// <summary>
        /// 是否开启上下文记录
        /// </summary>
        public static bool UseMessageContext { get; set; } = true;

        /// <summary>
        /// 每一个MessageContext过期时间（分钟），默认 30
        /// </summary>
        public static int ExpireMinutes { get; set; }
        /// <summary>
        /// 最大储存上下文数量（分别针对请求和响应信息），默认 20
        /// </summary>
        public static int MaxRecordCount { get; set; }

        static MessageContextGlobalConfig()
        {
            Restore();
        }

        public static void Restore()
        {
            ExpireMinutes = 30;
            MaxRecordCount = 20;
        }
    }


    /// <summary>
    /// 微信消息上下文操作对象（全局）
    /// 默认过期时间：90分钟
    /// </summary>
    public class GlobalMessageContext<TMC, TRequest, TResponse>
        where TMC : class, IMessageContext<TRequest, TResponse>, new() //TODO:TRequest, TResponse直接写明基类类型
        where TRequest : class, IRequestMessageBase
        where TResponse : class, IResponseMessageBase
    {

        private int _lastGlobalExpireMinutes;
        private int _expireMinutes;

        private int _lastGlobalMaxRecordCount;
        private int _maxRecordCount;

        /* 注意：1、如果 GlobalMessageContext 对象生命周期很多（比如再一次 MessageHandler 内），那么这里临时调整的意义不大，如果设为全局变量将有意义 
                 2、如果不用分布式缓存储存此结果，光使用静态变量，可能会导致线程间同步的问题
             */

        /// <summary>
        /// 每一个MessageContext过期时间（分钟）
        /// </summary>
        public int ExpireMinutes
        {
            get
            {
                if (_lastGlobalExpireMinutes != MessageContextGlobalConfig.ExpireMinutes)
                {
                    _expireMinutes = MessageContextGlobalConfig.ExpireMinutes;//全局参数已更新，当前上下文参数联动更新
                    _lastGlobalExpireMinutes = _expireMinutes;
                }
                return _expireMinutes;
            }
            set
            {
                _expireMinutes = value;
            }
        }

        /// <summary>
        /// 最大储存上下文数量（分别针对请求和响应信息）
        /// </summary>
        public int MaxRecordCount
        {
            get
            {
                if (_lastGlobalMaxRecordCount != MessageContextGlobalConfig.MaxRecordCount)
                {
                    _maxRecordCount = MessageContextGlobalConfig.MaxRecordCount;
                    _lastGlobalMaxRecordCount = _maxRecordCount;
                }
                return _maxRecordCount;
            }
            set
            {
                _maxRecordCount = value;
            }
        }


        public GlobalMessageContext()
        {
            ExpireMinutes = MessageContextGlobalConfig.ExpireMinutes;
            _lastGlobalExpireMinutes = ExpireMinutes;

            MaxRecordCount = MessageContextGlobalConfig.MaxRecordCount;
            _lastGlobalMaxRecordCount = MaxRecordCount;
        }

        private string GetCacheKey(string userName)
        {
            return $"{MessageContextGlobalConfig.CACHE_KEY_PREFIX}{userName}";
        }

        /// <summary>
        /// 获取过期时间 TimeSpan 对象
        /// </summary>
        /// <param name="expireMinutes"></param>
        /// <returns></returns>
        private TimeSpan? GetExpireTimeSpan(double? expireMinutes = null)
        {
            expireMinutes = expireMinutes ?? MessageContextGlobalConfig.ExpireMinutes;
            TimeSpan? expireTimeSpan = expireMinutes > 0 ? TimeSpan.FromMinutes(expireMinutes.Value) : (TimeSpan?)null;
            return expireTimeSpan;
        }

        #region 同步方法

        /// <summary>
        /// 重置所有上下文参数，所有记录将被清空（如果缓存数据比较多，性能开销将会比较大，请谨慎操作）
        /// </summary>
        public void Restore()
        {
            var cache = CacheStrategyFactory.GetObjectCacheStrategyInstance();

            //删除所有键
            var finalKeyPrefix = cache.GetFinalKey(GetCacheKey(""));
            var allObjects = cache.GetAll();
            var messageContextObjects = allObjects.Where(z => z.Key.StartsWith(finalKeyPrefix, StringComparison.Ordinal)).ToList();
            foreach (var item in messageContextObjects)
            {
                //Console.WriteLine($"{item.Key}");
                cache.RemoveFromCache(item.Key, true);//移除
            }

            ExpireMinutes = MessageContextGlobalConfig.ExpireMinutes;
            MaxRecordCount = MessageContextGlobalConfig.MaxRecordCount;
        }


        /// <summary>
        /// 获取MessageContext
        /// </summary>
        /// <param name="userName">用户名（OpenId）</param>
        /// <param name="createIfNotExists">true：如果用户不存在，则创建一个实例，并返回这个最新的实例
        /// false：如用户不存在，则返回null</param>
        /// <returns></returns>
        private TMC GetMessageContext(string userName, bool createIfNotExists)
        {
            var messageContext = GetMessageContext(userName);

            if (messageContext == null)
            {
                if (createIfNotExists)
                {
                    //全局只在这一个地方使用写入单用户上下文的原始对象
                    var newMessageContext = new TMC()
                    {
                        UserName = userName,
                        MaxRecordCount = MessageContextGlobalConfig.MaxRecordCount
                    };

                    var cache = CacheStrategyFactory.GetObjectCacheStrategyInstance();
                    var cacheKey = this.GetCacheKey(userName);
                    var expireTime = GetExpireTimeSpan();
                    cache.Set(cacheKey, newMessageContext, expireTime);//插入单用户上下文的原始缓存对象
                    //messageContext = GetMessageContext(userName);//注意！！这里如果使用Redis等分布式缓存立即从缓存读取，可能会因为还没有存入，发生为null的情况
                    messageContext = newMessageContext;
                }
                else
                {
                    return null;
                }
            }
            return messageContext;
        }

        /// <summary>
        /// 获取MessageContext，如果不存在，返回null
        /// 这个方法的更重要意义在于操作TM队列，及时移除过期信息，并将最新活动的对象移到尾部
        /// </summary>
        /// <param name="userName">用户名（OpenId）</param>
        /// <returns></returns>
        public TMC GetMessageContext(string userName)
        {
            //以下为新版本代码
            var cache = CacheStrategyFactory.GetObjectCacheStrategyInstance();
            using (cache.BeginCacheLock(MessageContextGlobalConfig.MESSAGE_CONTENT_ITEM_LOCK_NAME, $"GetMessageContext-{userName}"))
            {
                var cacheKey = this.GetCacheKey(userName);

                //注意：这里如果直接反序列化成 TMC，将无法保存类型，需要使用JsonConverter

                if (cache.CheckExisted(cacheKey))
                {
                    var cacheResult = cache.Get(cacheKey);

                    if (cacheResult == null)
                    {
                        return null;
                    }

                    if (cacheResult is TMC result)
                    {
                        return result;//比如使用内存缓存，此处会是原始对象
                    }

                    //TODO: 这里强制绑定 Newtonsoft 弹性并不好，后期必须进行分离！！！
                    if (cacheResult is Newtonsoft.Json.Linq.JObject jsonObj)
                    {
                        var jsonResult = JsonConvert.DeserializeObject<TMC>(jsonObj.ToString(), new MessageContextJsonConverter<TMC, TRequest, TResponse>());
                        //Console.WriteLine("从缓存读取result：\r\n" + jsonResult.ToJson(true));
                        return jsonResult;
                    }
                    else
                    {
                        throw new Exception("未知缓存对象，或未经注册的缓存框架");
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取MessageContext，如果不存在，使用requestMessage信息初始化一个，并返回原始实例
        /// </summary>
        /// <returns></returns>
        public TMC GetMessageContext(TRequest requestMessage)
        {
            if (requestMessage == null)
            {
                throw new NullReferenceException($"{nameof(requestMessage)} 不能为空");
            }
            return GetMessageContext(requestMessage.FromUserName, true);
        }

        /// <summary>
        /// 获取MessageContext，如果不存在，使用responseMessage信息初始化一个，并返回原始实例
        /// </summary>
        /// <returns></returns>
        public TMC GetMessageContext(TResponse responseMessage)
        {
            return GetMessageContext(responseMessage.ToUserName, true);
        }

        /// <summary>
        /// 记录请求信息
        /// </summary>
        /// <param name="requestMessage">请求信息</param>
        /// <param name="messageContext">上下文消息列表，如果为空，测自动从缓存中获取</param>
        public void InsertMessage(TRequest requestMessage, TMC messageContext = null)
        {
            if (requestMessage == null)
            {
                return;
            }

            var userName = requestMessage.FromUserName;
            var cache = CacheStrategyFactory.GetObjectCacheStrategyInstance();
            using (cache.BeginCacheLock(MessageContextGlobalConfig.MESSAGE_CONTENT_ITEM_LOCK_NAME, $"InsertMessage-{userName}"))
            {
                messageContext = messageContext ?? GetMessageContext(userName, true);
                //if (messageContext.RequestMessages.Count > 0)
                //{
                //    //如果不是新建的对象，把当前对象移到队列尾部（新对象已经在底部）
                //    var messageContextInQueue =
                //        MessageQueue.FindIndex(z => z.UserName == userName);

                //    if (messageContextInQueue >= 0)
                //    {
                //        MessageQueue.RemoveAt(messageContextInQueue); //移除当前对象
                //        MessageQueue.Add(messageContext); //插入到末尾
                //    }
                //}

                messageContext.LastActiveTime = messageContext.ThisActiveTime;//记录上一次请求时间
                messageContext.ThisActiveTime = SystemTime.Now;//记录本次请求时间

                //判断约束有没有修改
                if (messageContext.MaxRecordCount != this.MaxRecordCount)
                {
                    messageContext.MaxRecordCount = this.MaxRecordCount;
                    //messageContext.RequestMessages.MaxRecordCount = messageContext.MaxRecordCount;
                }

                messageContext.RequestMessages.Add(requestMessage);//录入消息（最大纪录限制会自动处理）

                var cacheKey = GetCacheKey(userName);
                var expireTime = GetExpireTimeSpan();
                cache.Set(cacheKey, messageContext, expireTime);

                //Console.WriteLine("Insert RequestMessage:\r\n"+ messageContext.ToJson(true));
            }
        }

        /// <summary>
        /// 记录响应信息
        /// </summary>
        /// <param name="responseMessage">响应信息</param>
        /// <param name="messageContext">上下文消息列表，如果为空，测自动从缓存中获取</param>
        public void InsertMessage(TResponse responseMessage, TMC messageContext = null)
        {
            if (responseMessage == null)
            {
                return;
            }

            var userName = responseMessage.ToUserName;
            var cache = CacheStrategyFactory.GetObjectCacheStrategyInstance();
            using (cache.BeginCacheLock(MessageContextGlobalConfig.MESSAGE_CONTENT_ITEM_LOCK_NAME, $"InsertMessage-{userName}"))
            {
                messageContext = messageContext ?? GetMessageContext(userName, true);

                //判断约束有没有修改
                if (messageContext.MaxRecordCount != this.MaxRecordCount)
                {
                    messageContext.MaxRecordCount = this.MaxRecordCount;
                }
                messageContext.ResponseMessages.Add(responseMessage);//录入消息（最大纪录限制会自动处理）

                var cacheKey = GetCacheKey(userName);
                var expireTime = GetExpireTimeSpan();

                cache.Set(cacheKey, messageContext, expireTime);
            }
        }

        /// <summary>
        /// 获取最新一条请求数据，如果不存在，则返回null
        /// </summary>
        /// <param name="userName">用户名（OpenId）</param>
        /// <returns></returns>
        public TRequest GetLastRequestMessage(string userName)
        {
            var cache = CacheStrategyFactory.GetObjectCacheStrategyInstance();
            using (cache.BeginCacheLock(MessageContextGlobalConfig.MESSAGE_CONTENT_ITEM_LOCK_NAME, $"GetMessageContext-{userName}"))
            {
                var messageContext = GetMessageContext(userName, true);
                return messageContext.RequestMessages.LastOrDefault();
            }
        }

        /// <summary>
        /// 获取最新一条响应数据，如果不存在，则返回null
        /// </summary>
        /// <param name="userName">用户名（OpenId）</param>
        /// <returns></returns>
        public TResponse GetLastResponseMessage(string userName)
        {
            var cache = CacheStrategyFactory.GetObjectCacheStrategyInstance();
            using (cache.BeginCacheLock(MessageContextGlobalConfig.MESSAGE_CONTENT_ITEM_LOCK_NAME, $"GetMessageContext-{userName}"))
            {
                var messageContext = GetMessageContext(userName, true);
                return messageContext.ResponseMessages.LastOrDefault();
            }
        }

        /// <summary>
        /// 更新上下文
        /// </summary>
        /// <param name="messageContext"></param>
        public void UpdateMessageContext(TMC messageContext)
        {
            var userName = messageContext.UserName;
            var cache = CacheStrategyFactory.GetObjectCacheStrategyInstance();
            using (cache.BeginCacheLock(MessageContextGlobalConfig.MESSAGE_CONTENT_ITEM_LOCK_NAME, $"UpdateMessageContext-{userName}"))
            {
                var cacheKey = GetCacheKey(userName);
                var expireTime = GetExpireTimeSpan();
                cache.Set(cacheKey, messageContext, expireTime);
            }
        }

        #endregion

        #region 异步方法

        /// <summary>
        /// 重置所有上下文参数，所有记录将被清空（如果缓存数据比较多，性能开销将会比较大，请谨慎操作）
        /// </summary>
        public async Task RestoreAsync()
        {
            var cache = CacheStrategyFactory.GetObjectCacheStrategyInstance();

            //删除所有键
            var finalKeyPrefix = cache.GetFinalKey(GetCacheKey(""));
            var allObjects = await cache.GetAllAsync().ConfigureAwait(false);
            var messageContextObjects = allObjects.Where(z => z.Key.StartsWith(finalKeyPrefix, StringComparison.Ordinal)).ToList();
            foreach (var item in messageContextObjects)
            {
                //Console.WriteLine($"{item.Key}");
                await cache.RemoveFromCacheAsync(item.Key, true).ConfigureAwait(false);//移除
            }

            ExpireMinutes = MessageContextGlobalConfig.ExpireMinutes;
            MaxRecordCount = MessageContextGlobalConfig.MaxRecordCount;
        }


        /// <summary>
        /// 获取MessageContext
        /// </summary>
        /// <param name="userName">用户名（OpenId）</param>
        /// <param name="createIfNotExists">true：如果用户不存在，则创建一个实例，并返回这个最新的实例
        /// false：如用户不存在，则返回null</param>
        /// <returns></returns>
        private async Task<TMC> GetMessageContextAsync(string userName, bool createIfNotExists)
        {
            var messageContext = await GetMessageContextAsync(userName).ConfigureAwait(false);

            if (messageContext == null)
            {
                if (createIfNotExists)
                {
                    //全局只在这一个地方使用写入单用户上下文的原始对象
                    var newMessageContext = new TMC()
                    {
                        UserName = userName,
                        MaxRecordCount = MessageContextGlobalConfig.MaxRecordCount
                    };

                    var cache = CacheStrategyFactory.GetObjectCacheStrategyInstance();
                    var cacheKey = this.GetCacheKey(userName);
                    var expireTime = GetExpireTimeSpan();
                    await cache.SetAsync(cacheKey, newMessageContext, expireTime).ConfigureAwait(false);//插入单用户上下文的原始缓存对象
                    //messageContext = GetMessageContext(userName);//注意！！这里如果使用Redis等分布式缓存立即从缓存读取，可能会因为还没有存入，发生为null的情况
                    messageContext = newMessageContext;
                }
                else
                {
                    return null;
                }
            }
            return messageContext;
        }

        /// <summary>
        /// 获取MessageContext，如果不存在，返回null
        /// 这个方法的更重要意义在于操作TM队列，及时移除过期信息，并将最新活动的对象移到尾部
        /// </summary>
        /// <param name="userName">用户名（OpenId）</param>
        /// <returns></returns>
        public async Task<TMC> GetMessageContextAsync(string userName)
        {
            //以下为新版本代码
            var cache = CacheStrategyFactory.GetObjectCacheStrategyInstance();

            using (await cache.BeginCacheLockAsync(MessageContextGlobalConfig.MESSAGE_CONTENT_ITEM_LOCK_NAME, $"GetMessageContext-{userName}").ConfigureAwait(false))
            {
                var cacheKey = this.GetCacheKey(userName);

                //注意：这里如果直接反序列化成 TMC，将无法保存类型，需要使用JsonConverter

                if (await cache.CheckExistedAsync(cacheKey).ConfigureAwait(false))
                {
                    var cacheResult = await cache.GetAsync(cacheKey).ConfigureAwait(false);

                    if (cacheResult == null)
                    {
                        return null;
                    }

                    if (cacheResult is TMC result)
                    {
                        return result;//比如使用内存缓存，此处会是原始对象
                    }

                    //TODO: 这里强制绑定 Newtonsoft 弹性并不好，后期必须进行分离！！！
                    if (cacheResult is Newtonsoft.Json.Linq.JObject jsonObj)
                    {
                        var jsonResult = JsonConvert.DeserializeObject<TMC>(jsonObj.ToString(), new MessageContextJsonConverter<TMC, TRequest, TResponse>());
                        //Console.WriteLine("从缓存读取result：\r\n" + jsonResult.ToJson(true));
                        return jsonResult;
                    }
                    else
                    {
                        throw new Exception("未知缓存对象，或未经注册的缓存框架");
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 获取MessageContext，如果不存在，使用requestMessage信息初始化一个，并返回原始实例
        /// </summary>
        /// <returns></returns>
        public async Task<TMC> GetMessageContextAsync(TRequest requestMessage)
        {
            if (requestMessage == null)
            {
                throw new NullReferenceException($"{nameof(requestMessage)} 不能为空");
            }
            return await GetMessageContextAsync(requestMessage.FromUserName, true).ConfigureAwait(false);
        }

        /// <summary>
        /// 获取MessageContext，如果不存在，使用responseMessage信息初始化一个，并返回原始实例
        /// </summary>
        /// <returns></returns>
        public async Task<TMC> GetMessageContextAsync(TResponse responseMessage)
        {
            return await GetMessageContextAsync(responseMessage.ToUserName, true).ConfigureAwait(false);
        }

        /// <summary>
        /// 记录请求信息
        /// </summary>
        /// <param name="requestMessage">请求信息</param>
        /// <param name="messageContext">上下文消息列表，如果为空，测自动从缓存中获取</param>
        public async Task InsertMessageAsync(TRequest requestMessage, TMC messageContext = null)
        {
            if (requestMessage == null)
            {
                return;
            }

            var userName = requestMessage.FromUserName;
            var cache = CacheStrategyFactory.GetObjectCacheStrategyInstance();
            using (await cache.BeginCacheLockAsync(MessageContextGlobalConfig.MESSAGE_CONTENT_ITEM_LOCK_NAME, $"InsertMessage-{userName}").ConfigureAwait(false))
            {
                messageContext = messageContext ?? await GetMessageContextAsync(userName, true).ConfigureAwait(false);
                //if (messageContext.RequestMessages.Count > 0)
                //{
                //    //如果不是新建的对象，把当前对象移到队列尾部（新对象已经在底部）
                //    var messageContextInQueue =
                //        MessageQueue.FindIndex(z => z.UserName == userName);

                //    if (messageContextInQueue >= 0)
                //    {
                //        MessageQueue.RemoveAt(messageContextInQueue); //移除当前对象
                //        MessageQueue.Add(messageContext); //插入到末尾
                //    }
                //}

                messageContext.LastActiveTime = messageContext.ThisActiveTime;//记录上一次请求时间
                messageContext.ThisActiveTime = SystemTime.Now;//记录本次请求时间

                //判断约束有没有修改
                if (messageContext.MaxRecordCount != this.MaxRecordCount)
                {
                    messageContext.MaxRecordCount = this.MaxRecordCount;
                    //messageContext.RequestMessages.MaxRecordCount = messageContext.MaxRecordCount;
                }

                messageContext.RequestMessages.Add(requestMessage);//录入消息（最大纪录限制会自动处理）

                var cacheKey = GetCacheKey(userName);
                var expireTime = GetExpireTimeSpan();
                await cache.SetAsync(cacheKey, messageContext, expireTime).ConfigureAwait(false);

                //Console.WriteLine("Insert RequestMessage:\r\n"+ messageContext.ToJson(true));
            }
        }

        /// <summary>
        /// 记录响应信息
        /// </summary>
        /// <param name="responseMessage">响应信息</param>
        /// <param name="messageContext">上下文消息列表，如果为空，测自动从缓存中获取</param>
        public async Task InsertMessageAsync(TResponse responseMessage, TMC messageContext = null)
        {
            if (responseMessage == null)
            {
                return;
            }

            var userName = responseMessage.ToUserName;
            var cache = CacheStrategyFactory.GetObjectCacheStrategyInstance();
            using (await cache.BeginCacheLockAsync(MessageContextGlobalConfig.MESSAGE_CONTENT_ITEM_LOCK_NAME, $"InsertMessage-{userName}").ConfigureAwait(false))
            {
                messageContext = messageContext ?? await GetMessageContextAsync(userName, true).ConfigureAwait(false);

                //判断约束有没有修改
                if (messageContext.MaxRecordCount != this.MaxRecordCount)
                {
                    messageContext.MaxRecordCount = this.MaxRecordCount;
                }
                messageContext.ResponseMessages.Add(responseMessage);//录入消息（最大纪录限制会自动处理）

                var cacheKey = GetCacheKey(userName);
                var expireTime = GetExpireTimeSpan();

                await cache.SetAsync(cacheKey, messageContext, expireTime).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 获取最新一条请求数据，如果不存在，则返回null
        /// </summary>
        /// <param name="userName">用户名（OpenId）</param>
        /// <returns></returns>
        public async Task<TRequest> GetLastRequestMessageAsync(string userName)
        {
            var cache = CacheStrategyFactory.GetObjectCacheStrategyInstance();
            using (await cache.BeginCacheLockAsync(MessageContextGlobalConfig.MESSAGE_CONTENT_ITEM_LOCK_NAME, $"GetMessageContext-{userName}").ConfigureAwait(false))
            {
                var messageContext = await GetMessageContextAsync(userName, true).ConfigureAwait(false);
                return messageContext.RequestMessages.LastOrDefault();
            }
        }

        /// <summary>
        /// 获取最新一条响应数据，如果不存在，则返回null
        /// </summary>
        /// <param name="userName">用户名（OpenId）</param>
        /// <returns></returns>
        public async Task<TResponse> GetLastResponseMessageAsync(string userName)
        {
            var cache = CacheStrategyFactory.GetObjectCacheStrategyInstance();
            using (await cache.BeginCacheLockAsync(MessageContextGlobalConfig.MESSAGE_CONTENT_ITEM_LOCK_NAME, $"GetMessageContext-{userName}").ConfigureAwait(false))
            {
                var messageContext = await GetMessageContextAsync(userName, true);
                return messageContext.ResponseMessages.LastOrDefault();
            }
        }

        /// <summary>
        /// 更新上下文
        /// </summary>
        /// <param name="messageContext"></param>
        public async Task UpdateMessageContextAsync(TMC messageContext)
        {
            var userName = messageContext.UserName;
            var cache = CacheStrategyFactory.GetObjectCacheStrategyInstance();
            using (await cache.BeginCacheLockAsync(MessageContextGlobalConfig.MESSAGE_CONTENT_ITEM_LOCK_NAME, $"UpdateMessageContext-{userName}").ConfigureAwait(false))
            {
                var cacheKey = GetCacheKey(userName);
                var expireTime = GetExpireTimeSpan();
                await cache.SetAsync(cacheKey, messageContext, expireTime).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
