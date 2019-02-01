using Senparc.NeuChar.Entities.App;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Senparc.NeuChar.App.Helpers
{
    /// <summary>
    /// 应用中枢 帮助类
    /// </summary>
    public class AppCentralisHelper
    {
        /// <summary>
        /// 获取全局变量指定键名（用于区分第三方App设置的常规参数）
        /// </summary>
        /// <param name="key">系统内（数据库内）设置的默认key</param>
        /// <returns></returns>
        public static string GetGlobalVariableKeyName(string key) => $"global.{key}";

        /// <summary>
        /// 获取指定参数，支持自动从全局变量获取
        /// </summary>
        /// <param name="inputPostModel"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetVariable(InputPostModel inputPostModel, string key)
        {
            var value = inputPostModel.PostData?.FirstOrDefault(z => z.Name == key)?.Value;
            if (value == null)
            {
                throw new Senparc.NeuChar.Exceptions.NeuCharException($"参数 {key} 为 null");
            }

            var regex = Regex.Match(value, @"^^\$\{(?<key>[^\}]+)\}$");//格式如：${appId}
            if (regex.Success)
            {
                //使用了全局变量
                var globalVariableKey = GetGlobalVariableKeyName(regex.Groups["key"].Value);
                value = GetVariable(inputPostModel, globalVariableKey);
            }

            return value;
        }
    }
}
