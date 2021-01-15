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
    
    文件名：NewtonSoft.Json.Extensions.cs
    文件功能描述：NewtonSoft.Json 的扩展方法
    
    
    创建标识：Senparc - 20200212
    

----------------------------------------------------------------*/

using Newtonsoft.Json.Linq;
using System;

namespace Senparc.NeuChar.Extensions
{
    /// <summary>
    /// NewtonSoft.Json 的扩展方法
    /// </summary>
    public static class NewtonSoft
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
        public static T TryGetValue<T>(this JObject obj, string propertyName, Action<JToken> action = null, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            if (obj.TryGetValue(propertyName, stringComparison, out JToken value))
            {
                action?.Invoke(value);

                if (value == null)
                {
                    return default(T);
                }

                if (value is JObject)
                {
                    return (value as JObject).ToObject<T>();
                }
                return value.Value<T>();
            }
            return default(T);
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
        public static object TryGetValue(this JObject obj, string propertyName, Type type, Action<JToken> action = null, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            if (obj.TryGetValue(propertyName, stringComparison, out JToken value))
            {
                action?.Invoke(value);

                if (value == null)
                {
                    return null;
                }
                return value.ToObject(type);
            }
            return null;
        }

    }
}
