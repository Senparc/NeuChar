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
    
    文件名：Register.cs
    文件功能描述：NeuChar 注册类
    
    
    创建标识：Senparc - 20180901
    
    修改标识：Senparc - 20190513
    修改描述：v0.6.7 
              1、添加 PushNeuCharAppConfig 和 PullNeuCharAppConfig 消息类型
              2、RegisterApiBind() 方法添加 forceBindAgain() 参数

----------------------------------------------------------------*/

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
        public static bool RegisterApiBindFinished { get; private set; } = false;

        /// <summary>
        /// 节点类型注册集合
        /// </summary>
        public static Dictionary<string, Type> NeuralNodeRegisterCollection = new Dictionary<string, Type>();
        //TODO: public static Dictionary<string, Type> NeuralNodeRegisterCollection { get; set; } = new Dictionary<string, Type>();


        static Register()
        {
            //RegisterApiBind(false);//注意：此处注册可能并不能获取到足够数量的程序集，需要测试并确定是否使用 RegisterApiBind(true) 方法

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
        /// RegisterApiBind 执行锁
        /// </summary>
        private static object RegisterApiBindLck = new object();

        /// <summary>
        /// 自动扫描并注册 ApiBind
        /// </summary>
        /// <param name="forceBindAgain">是否强制重刷新</param>
        public static void RegisterApiBind(bool forceBindAgain)
        {
            var dt1 = SystemTime.Now;

            //var cacheStragegy = CacheStrategyFactory.GetObjectCacheStrategyInstance();
            //using (cacheStragegy.BeginCacheLock("Senparc.NeuChar.Register", "RegisterApiBind"))
            lock (RegisterApiBindLck)//由于使用的是本地内存进行记录，所以这里不需要使用同步锁，这样就不需要依“缓存注册”等先决条件
            {
                if (RegisterApiBindFinished == true && forceBindAgain == false)
                {
                    Console.WriteLine($"RegisterApiBind has been finished, and doesn't require [forceBindAgain]. Quit build.");

                    return;
                }

                //查找所有扩展缓存
                var scanTypesCount = 0;

                var assembiles = AppDomain.CurrentDomain.GetAssemblies();

                var errorCount = 0;

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
                        errorCount++;
                        SenparcTrace.SendCustomLog("RegisterApiBind() 自动扫描程序集报告（非程序异常）：" + assembly.FullName, ex.ToString());
                    }
                }

                RegisterApiBindFinished = true;

                var dt2 = SystemTime.Now;
                Console.WriteLine($"RegisterApiBind Time: {(dt2 - dt1).TotalMilliseconds}ms, Api Count:{ApiBindInfoCollection.Instance.Count()}, Error Count: {errorCount}");
            }
        }
    }
}
