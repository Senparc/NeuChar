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
    
    文件名：NewtonSoft.Json.Extensions.cs
    文件功能描述：System.Text.Json DOM 扩展方法（保留旧类型名作为源码迁移入口）
    
    
    创建标识：Senparc - 20200212
    

----------------------------------------------------------------*/

using System;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Senparc.NeuChar.Extensions
{
    /// <summary>
    /// System.Text.Json DOM 扩展方法
    /// </summary>
    public static class SystemTextJsonExtensions
    {
        /// <summary>
        /// 从 JSON 对象中尝试获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="action"></param>
        /// <param name="stringComparison"></param>
        /// <returns></returns>
        public static T TryGetValue<T>(this JsonObject obj, string propertyName, Action<JsonNode> action = null, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            if (TryGetPropertyValue(obj, propertyName, stringComparison, out JsonNode value))
            {
                action?.Invoke(value);

                if (value == null)
                {
                    return default;
                }

                return value.Deserialize<T>(CreateSerializerOptions());
            }
            return default;
        }

        /// <summary>
        /// 从 JSON 对象中尝试获取值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="action"></param>
        /// <param name="stringComparison"></param>
        /// <returns></returns>
        public static object TryGetValue(this JsonObject obj, string propertyName, Type type, Action<JsonNode> action = null, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (TryGetPropertyValue(obj, propertyName, stringComparison, out JsonNode value))
            {
                action?.Invoke(value);

                if (value == null)
                {
                    return null;
                }
                return value.Deserialize(type, CreateSerializerOptions());
            }
            return null;
        }

        private static bool TryGetPropertyValue(JsonObject obj, string propertyName, StringComparison comparison, out JsonNode value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            foreach (var property in obj)
            {
                if (string.Equals(property.Key, propertyName, comparison))
                {
                    value = property.Value;
                    return true;
                }
            }

            value = null;
            return false;
        }

        private static JsonSerializerOptions CreateSerializerOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }
    }

    /// <summary>
    /// 兼容旧静态类名称。新代码请使用 <see cref="SystemTextJsonExtensions"/>。
    /// </summary>
    [Obsolete("Use SystemTextJsonExtensions with System.Text.Json.Nodes.JsonObject.")]
    public static class NewtonSoft
    {
        /// <summary>
        /// 从 JSON 对象中尝试获取值。
        /// </summary>
        public static T TryGetValue<T>(JsonObject obj, string propertyName, Action<JsonNode> action = null, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            return SystemTextJsonExtensions.TryGetValue<T>(obj, propertyName, action, stringComparison);
        }

        /// <summary>
        /// 从 JSON 对象中尝试获取指定类型的值。
        /// </summary>
        public static object TryGetValue(JsonObject obj, string propertyName, Type type, Action<JsonNode> action = null, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            return SystemTextJsonExtensions.TryGetValue(obj, propertyName, type, action, stringComparison);
        }
    }
}
