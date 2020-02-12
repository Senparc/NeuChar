using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senparc.NeuChar.Extensions
{
    /// <summary>
    /// NewtonSoft 扩展类
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
                return value.Value<T>();
            }
            return default(T);
        }

    }
}
