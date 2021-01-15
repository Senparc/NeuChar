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
    
    文件名：MessageContextJsonConverter.cs
    文件功能描述：Json 反序列化时用到的 JsonConverter
    
    
    创建标识：Senparc - 20190915
    

----------------------------------------------------------------*/

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Extensions;
using Senparc.NeuChar.NeuralSystems;
using System;
using System.Reflection;

namespace Senparc.NeuChar.Context
{
    /// <summary>
    /// Json 反序列化时用到的 JsonConverter
    /// </summary>
    public class MessageContextJsonConverter<TMC, TRequest, TResponse> : JsonConverter
        where TMC : class, IMessageContext<TRequest, TResponse>, new() //TODO:TRequest, TResponse直接写明基类类型
        where TRequest : class, IRequestMessageBase
        where TResponse : class, IResponseMessageBase
    {

        private DateTimeOffset? GetDateTimeOffset(JToken jToken)
        {
            DateTime? dateTime = jToken.Value<DateTime?>();
            if (!dateTime.HasValue)
            {
                return (DateTimeOffset?)null;
            }
            return DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IMessageContext<TRequest, TResponse>).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //参考：https://www.jerriepelser.com/blog/custom-converters-in-json-net-case-study-1/
            //Console.WriteLine("进入 ReadJson");

            if (reader.TokenType == JsonToken.StartObject)
            {
                //Console.WriteLine("进入 JsonToken.StartObject");

                JObject item = JObject.Load(reader);
                if (item.Count == 0)
                {
                    //没有对象，返回空
                    return null;
                }


                var messageContext = new TMC();
                try
                {
                    //TODO: 由于某些系统的全局设置，为 null 的参数可能会被忽略，因此需要对每一个参数进行存在性判断。

                    //messageContext.UserName = item["UserName"].Value<string>();
                    messageContext.UserName = item.TryGetValue<string>("UserName");//新方法，避免出现 null 异常
                    messageContext.LastActiveTime = GetDateTimeOffset(item["LastActiveTime"]);
                    messageContext.ThisActiveTime = GetDateTimeOffset(item["ThisActiveTime"]);
                    messageContext.ExpireMinutes = item.TryGetValue<Double?>("ExpireMinutes");
                    messageContext.AppStoreState = (AppStoreState)item.TryGetValue<int>("AppStoreState");
                    messageContext.CurrentAppDataItem = item.TryGetValue<AppDataItem>("CurrentAppDataItem");

                    messageContext.RequestMessages = new MessageContainer<TRequest>();
                    messageContext.ResponseMessages = new MessageContainer<TResponse>();
                    //MaxRecordCount 设置之后，会自动设置 RequestMessages 和 ResponseMessages内的对应参数
                    messageContext.MaxRecordCount = item.TryGetValue<int>("MaxRecordCount");

                    //StorageData 是比较特殊的，可以储存任何类型的参数
                    object storageData;
                    string storageDataType = item.TryGetValue<string>("StorageDataTypeName");
                    if (!string.IsNullOrEmpty(storageDataType))
                    {
                        //根据确定类型获取 StorageData
                        try
                        {
                            Type dataType = Type.GetType(storageDataType, true, true);
                            storageData = item.TryGetValue("StorageData", dataType);
                            messageContext.StorageDataType = dataType;
                        }
                        catch
                        {
                            storageData = item.TryGetValue<object>("StorageData");
                        }
                    }
                    else
                    {
                        storageData = item.TryGetValue<object>("StorageData");
                    }

                    if (storageData is JValue jv)
                    {
                        messageContext.StorageData = jv.Value;
                    }
                    else
                    {
                        messageContext.StorageData = storageData;
                    }

                    //RequestMessages 和 ResponseMessages 节点内容比较稳定，暂不使用 JObject.TryGetValue<T>() 方法
                    if (item["RequestMessages"] != null)
                    {
                        foreach (var requestMessage in item["RequestMessages"].Children())
                        {
                            var msgType = (RequestMsgType)requestMessage["MsgType"].Value<int>();
                            var doc = JsonConvert.DeserializeXNode(requestMessage.ToString(), "xml");
                            var emptyEntity = messageContext.GetRequestEntityMappingResult(msgType, doc);//获取空对象
                            var filledEntity = requestMessage.ToObject(emptyEntity.GetType()) as TRequest;
                            if (filledEntity != null)
                            {
                                messageContext.RequestMessages.Add(filledEntity);
                            }
                        }
                    }

                    if (item["ResponseMessages"] != null)
                    {
                        foreach (var responseMessage in item["ResponseMessages"].Children())
                        {
                            var msgType = (ResponseMsgType)responseMessage["MsgType"].Value<int>();
                            var doc = JsonConvert.DeserializeXNode(responseMessage.ToString(), "xml");
                            var emptyEntity = messageContext.GetResponseEntityMappingResult(msgType, doc);//获取空对象
                            var filledEntity = responseMessage.ToObject(emptyEntity.GetType()) as TResponse;
                            if (filledEntity != null)
                            {
                                messageContext.ResponseMessages.Add(filledEntity);
                            }
                        }
                    }

                }
                catch/* (Exception ex)*/
                {
                    //此处可以进行调试跟踪
                    throw;
                }
                return messageContext;
            }
            else
            {
                //JArray array = JArray.Load(reader);

                //var users = array.ToObject<IList<User>>();

                //return new PagedList<User>(users);

                Console.WriteLine("进入非 JsonToken.StartObject");
                var messageContext = new TMC();
                messageContext.UserName = "ELSE";
                return messageContext;
            }

            // This should not happen. Perhaps better to throw exception at this point?
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new System.NotImplementedException();
        }
    }
}
