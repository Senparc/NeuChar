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

            var apiBindList = new Dictionary<string, MethodInfo>();

            var types = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(a =>
                        {
                            try
                            {
                                scanTypesCount++;
                                var aTypes = a.GetTypes();

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
                                            apiBindList.Add(apiBindAttr.Name, )
                                        }

                                    }



                                }

                                var filtedType = aTypes.Where(t => !t.IsAbstract &&
                                                          t.IsPublic &&
                                                          t.GetCustomAttributes(typeof(ApiBindAttribute), true).Count() > 0);



                                foreach (var type in aTypes)
                                {

                                }

                                foreach (var type in aTypes)
                                {
                                    var apiBindAttrs = System.Attribute.GetCustomAttributes(type).Where(z => z is ApiBindAttribute)
                                                                        .Select(z => z as ApiBindAttribute).ToList();
                                    foreach (var apiBindAttr in apiBindAttrs)
                                    {
                                        var name = $"{apiBindAttr.Name}";//TODO：生成全局唯一名称
                                        var s = apiBindAttr.
                                        apiBindList.Add()
                                    }
                                }

                                return aTypes.Where(t => !t.IsAbstract &&
                                                          t.IsPublic &&
                                                          t.GetCustomAttributes(typeof(ApiBindAttribute), true).Count() > 0);
                            }
                            catch (Exception ex)
                            {
                                SenparcTrace.SendCustomLog("RegisterApiBind() 自动扫描程序集异常：" + a.FullName, ex.ToString());
                                return new List<Type>();//不能 return null
                            }
                        });

            if (types != null)
            {
                foreach (var type in types)
                {
                    if (type == null)
                    {
                        continue;
                    }
                    try
                    {
                        var apiBindAttr =

                        var exCache = ReflectionHelper.GetStaticMember(type, "Instance");

                        cacheTypes += "\r\n" + type;//由于数量不多，这里使用String，不使用StringBuilder
                        addedTypes.Add(type);
                    }
                    catch (Exception ex)
                    {
                        Trace.SenparcTrace.BaseExceptionLog(new Exceptions.BaseException(ex.Message, ex));
                    }
                }
            }
        }

    }
}
}
