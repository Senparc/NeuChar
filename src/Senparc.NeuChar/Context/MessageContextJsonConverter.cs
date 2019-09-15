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
    public class MessageContextJsonConverter<TMC,TRequest, TResponse> : JsonConverter
        where TMC : class, IMessageContext<TRequest, TResponse>, new() //TODO:TRequest, TResponse直接写明基类类型
        where TRequest : class, IRequestMessageBase
        where TResponse : class, IResponseMessageBase
    {

        private DateTimeOffset? GetDateTimeOffset(JToken jToken) {
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
            Console.WriteLine("进入 ReadJson");

            if (reader.TokenType == JsonToken.StartObject)
            {
                Console.WriteLine("进入 JsonToken.StartObject");
                
                JObject item = JObject.Load(reader);
                Console.WriteLine("JObject item:"+ item.ToJson()+$" Count：{item.Count}");
                if (item.Count==0)
                {
                    //没有对象，返回空
                    return null;
                }

                var messageContext = new TMC();
                messageContext.UserName = item["UserName"].Value<string>();
                messageContext.LastActiveTime = GetDateTimeOffset(item["LastActiveTime"]);
                messageContext.ThisActiveTime = GetDateTimeOffset(item["ThisActiveTime"]);
                messageContext.MaxRecordCount = item["MaxRecordCount"].Value<int>();
                messageContext.StorageData = item["StorageData"].Value<object>();
                messageContext.ExpireMinutes = item["ExpireMinutes"].Value<Double?>();
                messageContext.AppStoreState = (AppStoreState)(item["AppStoreState"].Value<int>());
                messageContext.CurrentAppDataItem = item["CurrentAppDataItem"].Value<AppDataItem>();

                if (item["RequestMessages"] != null)
                {
                    Console.WriteLine("RequestMessage is not null");

                    messageContext.RequestMessages = new MessageContainer<TRequest>();
                    foreach (var requestMessage in item["RequestMessages"].Children()) {
                        var msgType = (RequestMsgType)requestMessage["MsgType"].Value<int>();
                        var emptyEntity = messageContext.GetRequestEntityMappingResult(msgType);//获取空对象
                        var filledEntity = requestMessage.ToObject(emptyEntity.GetType()) as TRequest;
                        if (filledEntity!=null)
                        {
                            messageContext.RequestMessages.Add(filledEntity);
                        }
                    }

                    //var users = item["users"].ToObject<IList<User>>(serializer);

                    //int length = item["length"].Value<int>();
                    //int limit = item["limit"].Value<int>();
                    //int start = item["start"].Value<int>();
                    //int total = item["total"].Value<int>();

                    //return new PagedList<User>(users, new PagingInformation(start, limit, length, total));
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
