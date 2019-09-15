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
        public override bool CanConvert(Type objectType)
        {
            return typeof(IMessageContext<TRequest, TResponse>).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
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
                messageContext.LastActiveTime = item["LastActiveTime"].Value<DateTimeOffset?>();
                messageContext.ThisActiveTime = item["ThisActiveTime"].Value<DateTimeOffset?>();
                messageContext.MaxRecordCount = item["MaxRecordCount"].Value<int>();
                messageContext.StorageData = item["StorageData"].Value<object>();
                messageContext.ExpireMinutes = item["ExpireMinutes"].Value<Double?>();
                messageContext.AppStoreState = item["AppStoreState"].Value<AppStoreState>();
                messageContext.CurrentAppDataItem = item["CurrentAppDataItem"].Value<AppDataItem>();


                if (item["RequestMessage"] != null)
                {
                    Console.WriteLine("RequestMessage is not null");
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
