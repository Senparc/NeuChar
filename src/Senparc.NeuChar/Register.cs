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
    
    文件名：Register.cs
    文件功能描述：NeuChar 注册类
    
    
    创建标识：Senparc - 20180901
    
    修改标识：Senparc - 20190513
    修改描述：v0.6.7 
              1、添加 PushNeuCharAppConfig 和 PullNeuCharAppConfig 消息类型
              2、RegisterApiBind() 方法添加 forceBindAgain() 参数

    修改标识：Senparc - 20210705
    修改描述：v1.5 重构到 CO2NET 的 WebApiEngine

    修改标识：Senparc - 20230614
    修改描述：v2.1.7.1 添加 IgnoreNeuCharApiBind 属性

----------------------------------------------------------------*/

using Senparc.NeuChar.NeuralSystems;
using System;
using System.Collections.Generic;
#if !NET462
using Microsoft.Extensions.DependencyInjection;
#endif
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
        public static Dictionary<string, Type> NeuralNodeRegisterCollection { get; set; } = new Dictionary<string, Type>();
        //TODO: public static Dictionary<string, Type> NeuralNodeRegisterCollection { get; set; } = new Dictionary<string, Type>();

        /// <summary>
        /// 是否忽略所有 NeuChar 的 ApiBind 特性
        /// </summary>
        public static bool IgnoreNeuCharApiBind { get; set; } = true;

        static Register()
        {
            //RegisterApiBind(false);//注意：此处注册可能并不能获取到足够数量的程序集，需要测试并确定是否使用 RegisterApiBind(true) 方法

            //注册节点类型
            RegisterNeuralNode("MessageHandlerNode", typeof(MessageHandlerNode));
            RegisterNeuralNode("AppDataNode", typeof(AppDataNode));
        }

        /// <summary>
        /// 注册 NeuChar
        /// </summary>
        /// <returns></returns>
        public static void AddNeuChar(bool ignoreNeuCharApiBind = true)
        {
            IgnoreNeuCharApiBind = ignoreNeuCharApiBind;
        }

#if !NET462
        /// <summary>
        /// 注册 NeuChar
        /// </summary>
        /// <param name="services"></param>
        /// <param name="ignoreNeuCharApiBind">是否统一忽略生成所有 NeuChar WebApi</param>
        /// <returns></returns>
        public static IServiceCollection AddNeuChar(this IServiceCollection services, bool ignoreNeuCharApiBind = true)
        {
            AddNeuChar(ignoreNeuCharApiBind);

            return services;
        }
#endif

        /// <summary>
        /// 注册节点
        /// </summary>
        /// <param name="name">唯一名称</param>
        /// <param name="type">节点类型</param>
        public static void RegisterNeuralNode(string name, Type type)
        {
            NeuralNodeRegisterCollection[name] = type;
        }
    }
}
