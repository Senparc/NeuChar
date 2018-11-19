using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Senparc.NeuChar.ApiBind
{
    /// <summary>
    /// ApiBind 绑定信息的全局唯一集合
    /// </summary>
    public class ApiBindInfoCollection : Dictionary<string, ApiBindInfo>
    {
        #region 单例

        //静态SearchCache
        public static ApiBindInfoCollection Instance
        {
            get
            {
                return Nested.instance;//返回Nested类中的静态成员instance
            }
        }

        class Nested
        {
            static Nested()
            {
            }
            //将instance设为一个初始化的BaseCacheStrategy新实例
            internal static readonly ApiBindInfoCollection instance = new ApiBindInfoCollection();
        }

        #endregion

        /// <summary>
        /// 获取全局唯一名称
        /// </summary>
        /// <param name="platformType">PlatformType</param>
        /// <param name="apiBindAttrName">跨程序集的通用名称（如：CustomApi.SendText）</param>
        /// <returns></returns>
        private string GetGlobalName(PlatformType platformType, string apiBindAttrName)
        {
            return $"{platformType.ToString()}:{apiBindAttrName}";//TODO：生成全局唯一名称
        }

        /// <summary>
        /// ApiBindCollection 构造函数
        /// </summary>
        public ApiBindInfoCollection() : base(StringComparer.OrdinalIgnoreCase)
        {

        }

        /// <summary>
        /// 添加 ApiBindInfo 对象
        /// </summary>
        /// <param name="method"></param>
        /// <param name="apiBindAttr"></param>
        public void Add(MethodInfo method, ApiBindAttribute apiBindAttr)
        {
            var name = GetGlobalName(apiBindAttr.PlatformType, apiBindAttr.Name);

            var finalName = name;
            var suffix = 0;
            //确保名称不会重复
            while (base.ContainsKey(finalName))
            {
                suffix++;
                finalName = name + suffix.ToString("00");
            }

            base.Add(finalName, new ApiBindInfo(apiBindAttr, method));
        }

        /// <summary>
        /// 获取 ApiBindInfo
        /// </summary>
        /// <param name="platformType">PlatformType</param>
        /// <param name="apiBindAttrName">跨程序集的通用名称（如：CustomApi.SendText）</param>
        public ApiBindInfo Get(PlatformType platformType, string apiBindAttrName)
        {
            var name = GetGlobalName(platformType, apiBindAttrName);
            if (ApiBindInfoCollection.Instance.ContainsKey(name))
            {
                return ApiBindInfoCollection.Instance[name];
            }
            return null;
        }
    }
}
