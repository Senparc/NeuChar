using Senparc.CO2NET.Trace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Senparc.NeuChar
{
    /// <summary>
    /// NeuChar 注册
    /// </summary>
    public static class Register
    {
        /// <summary>
        /// 节点类型注册集合
        /// </summary>
        public static Dictionary<string, Type> NeuralNodeRegisterCollection = new Dictionary<string, Type>();

        /// <summary>
        /// 注册节点
        /// </summary>
        /// <param name="name">唯一名称</param>
        /// <param name="type">节点类型</param>
        public static void RegisterNeuralNode(string name, Type type)
        {
            NeuralNodeRegisterCollection[name] = type;
        }

        /// <summary>
        /// 自动扫描并注册 ApiBind
        /// </summary>
        public static void RegisterApiBind()
        {
            //查找所有扩展缓存
            var scanTypesCount = 0;

            var apiBindList = new Dictionary<string, ApiBindInfo>();

            var assembiles = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assembiles)
            {
                try
                {
                    scanTypesCount++;
                    var aTypes = assembly.GetTypes();

                    foreach (var type in aTypes)
                    {
                        if (type.IsAbstract || !type.IsPublic)
                        {
                            continue;
                        }

                        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod);
                        foreach (var method in methods)
                        {
                            var attrs = method.GetCustomAttributes(typeof(ApiBindAttribute), true);
                            foreach (var attr in attrs)
                            {
                                var apiBindAttr = attr as ApiBindAttribute;
                                var name = $"{apiBindAttr.Name}";//TODO：生成全局唯一名称
                                apiBindList.Add(name, new ApiBindInfo(apiBindAttr, method));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    SenparcTrace.SendCustomLog("RegisterApiBind() 自动扫描程序集异常：" + assembly.FullName, ex.ToString());
                }
            }
        }
    }
}
