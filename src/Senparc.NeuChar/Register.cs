using Senparc.CO2NET.Cache;
using Senparc.CO2NET.Trace;
using Senparc.NeuChar.ApiBind;
using Senparc.NeuChar.NeuralSystems;
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
        /// 是否API绑定已经执行完
        /// </summary>
        private static bool RegisterApiBindFinished = false;


        /// <summary>
        /// 节点类型注册集合
        /// </summary>
        public static Dictionary<string, Type> NeuralNodeRegisterCollection = new Dictionary<string, Type>();


        static Register()
        {
            RegisterApiBind();

            //注册节点类型
           RegisterNeuralNode("MessageHandlerNode", typeof(MessageHandlerNode));
           RegisterNeuralNode("AppDataNode", typeof(AppDataNode));
        }


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
            var dt1 = SystemTime.Now;

            var cacheStragegy = CacheStrategyFactory.GetObjectCacheStrategyInstance();
            using (cacheStragegy.BeginCacheLock("Senparc.NeuChar.Register", "RegisterApiBind"))
            {
                if (RegisterApiBindFinished == true)
                {
                    return;
                }

                //查找所有扩展缓存
                var scanTypesCount = 0;

                var assembiles = AppDomain.CurrentDomain.GetAssemblies();

                foreach (var assembly in assembiles)
                {
                    try
                    {
                        scanTypesCount++;
                        var classTypes = assembly.GetTypes()
                                    .Where(z => z.Name.EndsWith("api", StringComparison.OrdinalIgnoreCase) ||
                                                z.Name.EndsWith("apis", StringComparison.OrdinalIgnoreCase))
                                    .ToArray();

                        foreach (var type in classTypes)
                        {
                            if (/*type.IsAbstract || 静态类会被识别为 IsAbstract*/
                                !type.IsPublic || !type.IsClass || type.IsEnum)
                            {
                                continue;
                            }

                            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod);
                            foreach (var method in methods)
                            {
                                var attrs = method.GetCustomAttributes(typeof(ApiBindAttribute), false);
                                foreach (var attr in attrs)
                                {
                                    ApiBindInfoCollection.Instance.Add(method, attr as ApiBindAttribute);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        SenparcTrace.SendCustomLog("RegisterApiBind() 自动扫描程序集报告（非程序异常）：" + assembly.FullName, ex.ToString());
                    }
                }

                RegisterApiBindFinished = true;

                var dt2 = SystemTime.Now;
                Console.WriteLine($"RegisterApiBind 用时：{(dt2 - dt1).TotalMilliseconds}ms");
            }
        }
    }
}
