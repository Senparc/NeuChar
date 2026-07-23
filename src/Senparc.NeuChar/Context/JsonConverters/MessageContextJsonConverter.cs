#region Apache License Version 2.0
/*----------------------------------------------------------------

Copyright 2026 Suzhou Senparc Network Technology Co.,Ltd.

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
    Copyright (C) 2026 Senparc

    文件名：MessageContextJsonConverter.cs
    文件功能描述：使用 System.Text.Json 还原包含多态消息的 MessageContext


    创建标识：Senparc - 20190915

    修改标识：Senparc - 20260723
    修改描述：v3.0.0 迁移至 System.Text.Json，并兼容 CO2NET 4.x 的动态缓存对象

----------------------------------------------------------------*/

using Senparc.CO2NET.Helpers.Serializers;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.NeuralSystems;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Linq;

namespace Senparc.NeuChar.Context
{
    /// <summary>
    /// 使用 System.Text.Json 反序列化包含多态请求、响应消息的上下文。
    /// </summary>
    public class MessageContextJsonConverter<TMC, TRequest, TResponse> : JsonConverter<TMC>
        where TMC : class, IMessageContext<TRequest, TResponse>, new()
        where TRequest : class, IRequestMessageBase
        where TResponse : class, IResponseMessageBase
    {
        /// <summary>
        /// 使用默认的 Senparc JSON 兼容设置反序列化消息上下文。
        /// </summary>
        public static TMC Deserialize(string json, JsonSerializerOptions options = null)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            return JsonSerializer.Deserialize<TMC>(json, CreateSerializerOptions(options));
        }

        /// <summary>
        /// 创建包含当前转换器的 JSON 设置。
        /// </summary>
        public static JsonSerializerOptions CreateSerializerOptions(JsonSerializerOptions options = null)
        {
            var result = options == null
                ? new JsonSerializerOptions(new JsonSettingWrap().Options)
                : new JsonSerializerOptions(options);

            result.PropertyNameCaseInsensitive = true;
            if (!ContainsCurrentConverter(result))
            {
                result.Converters.Add(new MessageContextJsonConverter<TMC, TRequest, TResponse>());
            }

            return result;
        }

        /// <inheritdoc />
        public override TMC Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (var document = JsonDocument.ParseValue(ref reader))
            {
                var item = document.RootElement;
                if (item.ValueKind == JsonValueKind.Null)
                {
                    return null;
                }

                if (item.ValueKind != JsonValueKind.Object)
                {
                    throw new JsonException($"MessageContext JSON root must be an object, actual kind: {item.ValueKind}.");
                }

                return ReadMessageContext(item, options);
            }
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer, TMC value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            var writeOptions = new JsonSerializerOptions(options);
            for (var i = writeOptions.Converters.Count - 1; i >= 0; i--)
            {
                if (writeOptions.Converters[i] is MessageContextJsonConverter<TMC, TRequest, TResponse>)
                {
                    writeOptions.Converters.RemoveAt(i);
                }
            }

            JsonSerializer.Serialize(writer, value, value.GetType(), writeOptions);
        }

        private static TMC ReadMessageContext(JsonElement item, JsonSerializerOptions options)
        {
            var messageContext = new TMC
            {
                AppId = DeserializeProperty<string>(item, "AppId", options),
                UserName = DeserializeProperty<string>(item, "UserName", options),
                LastActiveTime = DeserializeDateTimeOffset(item, "LastActiveTime", options),
                ThisActiveTime = DeserializeDateTimeOffset(item, "ThisActiveTime", options),
                ExpireMinutes = DeserializeProperty<double?>(item, "ExpireMinutes", options),
                AppStoreState = DeserializeProperty<AppStoreState>(item, "AppStoreState", options),
                CurrentAppDataItem = DeserializeProperty<AppDataItem>(item, "CurrentAppDataItem", options),
                RequestMessages = new MessageContainer<TRequest>(),
                ResponseMessages = new MessageContainer<TResponse>()
            };

            messageContext.MaxRecordCount = DeserializeProperty<int>(item, "MaxRecordCount", options);
            RestoreStorageData(messageContext, item, options);
            RestoreRequestMessages(messageContext, item, options);
            RestoreResponseMessages(messageContext, item, options);

            return messageContext;
        }

        private static void RestoreStorageData(TMC messageContext, JsonElement item, JsonSerializerOptions options)
        {
            if (!TryGetProperty(item, "StorageData", out var storageElement) ||
                storageElement.ValueKind == JsonValueKind.Null ||
                storageElement.ValueKind == JsonValueKind.Undefined)
            {
                messageContext.StorageData = null;
                return;
            }

            var storageDataTypeName = DeserializeProperty<string>(item, "StorageDataTypeName", options);
            var storageDataType = ResolveType(storageDataTypeName);
            if (storageDataType != null)
            {
                try
                {
                    messageContext.StorageData = JsonSerializer.Deserialize(storageElement.GetRawText(), storageDataType, options);
                    messageContext.StorageDataType = storageDataType;
                    return;
                }
                catch (JsonException)
                {
                    // 兼容历史缓存：类型已经移动或旧 JSON 形状与当前类型不一致时，回退为动态值。
                }
            }

            messageContext.StorageData = ConvertElement(storageElement);
        }

        private static void RestoreRequestMessages(TMC messageContext, JsonElement item, JsonSerializerOptions options)
        {
            if (!TryGetProperty(item, "RequestMessages", out var messages) || messages.ValueKind != JsonValueKind.Array)
            {
                return;
            }

            foreach (var requestMessage in messages.EnumerateArray())
            {
                if (!TryGetProperty(requestMessage, "MsgType", out var msgTypeElement))
                {
                    continue;
                }

                var msgType = JsonSerializer.Deserialize<RequestMsgType>(msgTypeElement.GetRawText(), options);
                var emptyEntity = messageContext.GetRequestEntityMappingResult(msgType, ConvertToXDocument(requestMessage));
                if (emptyEntity == null)
                {
                    continue;
                }

                var filledEntity = JsonSerializer.Deserialize(requestMessage.GetRawText(), emptyEntity.GetType(), options) as TRequest;
                if (filledEntity != null)
                {
                    messageContext.RequestMessages.Add(filledEntity);
                }
            }
        }

        private static void RestoreResponseMessages(TMC messageContext, JsonElement item, JsonSerializerOptions options)
        {
            if (!TryGetProperty(item, "ResponseMessages", out var messages) || messages.ValueKind != JsonValueKind.Array)
            {
                return;
            }

            foreach (var responseMessage in messages.EnumerateArray())
            {
                if (!TryGetProperty(responseMessage, "MsgType", out var msgTypeElement))
                {
                    continue;
                }

                var msgType = JsonSerializer.Deserialize<ResponseMsgType>(msgTypeElement.GetRawText(), options);
                var emptyEntity = messageContext.GetResponseEntityMappingResult(msgType, ConvertToXDocument(responseMessage));
                if (emptyEntity == null)
                {
                    continue;
                }

                var filledEntity = JsonSerializer.Deserialize(responseMessage.GetRawText(), emptyEntity.GetType(), options) as TResponse;
                if (filledEntity != null)
                {
                    messageContext.ResponseMessages.Add(filledEntity);
                }
            }
        }

        private static T DeserializeProperty<T>(JsonElement item, string propertyName, JsonSerializerOptions options)
        {
            if (!TryGetProperty(item, propertyName, out var value) ||
                value.ValueKind == JsonValueKind.Null ||
                value.ValueKind == JsonValueKind.Undefined)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(value.GetRawText(), options);
        }

        private static DateTimeOffset? DeserializeDateTimeOffset(JsonElement item, string propertyName, JsonSerializerOptions options)
        {
            if (!TryGetProperty(item, propertyName, out var value) ||
                value.ValueKind == JsonValueKind.Null ||
                value.ValueKind == JsonValueKind.Undefined)
            {
                return null;
            }

            if (value.ValueKind == JsonValueKind.String && value.TryGetDateTimeOffset(out var dateTimeOffset))
            {
                return dateTimeOffset;
            }

            return JsonSerializer.Deserialize<DateTimeOffset?>(value.GetRawText(), options);
        }

        private static bool TryGetProperty(JsonElement item, string propertyName, out JsonElement value)
        {
            if (item.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in item.EnumerateObject())
                {
                    if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        value = property.Value;
                        return true;
                    }
                }
            }

            value = default;
            return false;
        }

        private static Type ResolveType(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                return null;
            }

            var type = Type.GetType(typeName, false, true);
            if (type != null)
            {
                return type;
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(typeName, false, true);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }

        private static object ConvertElement(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    var dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                    foreach (var property in element.EnumerateObject())
                    {
                        dictionary[property.Name] = ConvertElement(property.Value);
                    }
                    return dictionary;
                case JsonValueKind.Array:
                    var list = new List<object>();
                    foreach (var child in element.EnumerateArray())
                    {
                        list.Add(ConvertElement(child));
                    }
                    return list;
                case JsonValueKind.String:
                    if (element.TryGetDateTimeOffset(out var dateTimeOffset))
                    {
                        return dateTimeOffset;
                    }
                    return element.GetString();
                case JsonValueKind.Number:
                    if (element.TryGetInt64(out var integer))
                    {
                        return integer;
                    }
                    if (element.TryGetDecimal(out var decimalValue))
                    {
                        return decimalValue;
                    }
                    return element.GetDouble();
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    return null;
                default:
                    throw new JsonException($"Unsupported JSON token: {element.ValueKind}.");
            }
        }

        private static XDocument ConvertToXDocument(JsonElement element)
        {
            var root = new XElement("xml");
            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in element.EnumerateObject())
                {
                    root.Add(ConvertToXElement(property.Name, property.Value));
                }
            }
            return new XDocument(root);
        }

        private static XElement ConvertToXElement(string name, JsonElement element)
        {
            var safeName = XmlConvert.EncodeLocalName(name);
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    var objectElement = new XElement(safeName);
                    foreach (var property in element.EnumerateObject())
                    {
                        objectElement.Add(ConvertToXElement(property.Name, property.Value));
                    }
                    return objectElement;
                case JsonValueKind.Array:
                    var arrayElement = new XElement(safeName);
                    foreach (var child in element.EnumerateArray())
                    {
                        arrayElement.Add(ConvertToXElement("item", child));
                    }
                    return arrayElement;
                case JsonValueKind.String:
                    return new XElement(safeName, element.GetString());
                case JsonValueKind.Number:
                    return new XElement(safeName, element.GetRawText());
                case JsonValueKind.True:
                    return new XElement(safeName, bool.TrueString.ToLowerInvariant());
                case JsonValueKind.False:
                    return new XElement(safeName, bool.FalseString.ToLowerInvariant());
                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                    return new XElement(safeName);
                default:
                    return new XElement(safeName, Convert.ToString(ConvertElement(element), CultureInfo.InvariantCulture));
            }
        }

        private static bool ContainsCurrentConverter(JsonSerializerOptions options)
        {
            foreach (var converter in options.Converters)
            {
                if (converter is MessageContextJsonConverter<TMC, TRequest, TResponse>)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
