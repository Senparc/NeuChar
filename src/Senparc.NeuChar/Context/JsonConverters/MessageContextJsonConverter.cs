using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Senparc.CO2NET.Extensions;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.NeuralSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

                    messageContext.UserName = item["UserName"].Value<string>();
                    messageContext.LastActiveTime = GetDateTimeOffset(item["LastActiveTime"]);
                    messageContext.ThisActiveTime = GetDateTimeOffset(item["ThisActiveTime"]);
                    messageContext.ExpireMinutes = item["ExpireMinutes"].Value<Double?>();
                    messageContext.AppStoreState = (AppStoreState)(item["AppStoreState"].Value<int>());
                    messageContext.CurrentAppDataItem = item["CurrentAppDataItem"].ToObject<AppDataItem>();

                    messageContext.RequestMessages = new MessageContainer<TRequest>();
                    messageContext.ResponseMessages = new MessageContainer<TResponse>();
                    //MaxRecordCount 设置之后，会自动设置 RequestMessages 和 ResponseMessages内的对应参数
                    messageContext.MaxRecordCount = item["MaxRecordCount"].Value<int>();

                    //StorageData 是比较特殊的，可以储存任何类型的参数
                    object storageData;
                    string storageDataType = item["StorageDataTypeName"].Value<string>();
                    if (!string.IsNullOrEmpty(storageDataType))
                    {
                        //根据确定类型获取 StorageData
                        try
                        {
                            Type dataType = Type.GetType(storageDataType, true, true);
                            storageData = item["StorageData"].ToObject(dataType);
                            messageContext.StorageDataType = dataType;
                        }
                        catch
                        {
                            storageData = item["StorageData"].Value<object>();
                        }
                    }
                    else
                    {
                        storageData = item["StorageData"].Value<object>();
                    }

                    if (storageData is JValue jv)
                    {
                        messageContext.StorageData = jv.Value;
                    }
                    else
                    {
                        messageContext.StorageData = storageData;
                    }

                    if (item["RequestMessages"] != null)
                    {
                        foreach (var requestMessage in item["RequestMessages"].Children())
                        {
                            var msgType = (RequestMsgType)requestMessage["MsgType"].Value<int>();
                            var emptyEntity = messageContext.GetRequestEntityMappingResult(msgType);//获取空对象
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
                            var emptyEntity = messageContext.GetResponseEntityMappingResult(msgType);//获取空对象
                            var filledEntity = responseMessage.ToObject(emptyEntity.GetType()) as TResponse;
                            if (filledEntity != null)
                            {
                                messageContext.ResponseMessages.Add(filledEntity);
                            }
                        }
                    }

                }
                catch (Exception ex)
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

                Console.WriteLine("进入 非 JsonToken.StartObject");
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
