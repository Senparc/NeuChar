﻿#region Apache License Version 2.0
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
    
    文件名：NeuralSystem.cs
    文件功能描述：神经系统，整个系统数据的根节点
    
    
    创建标识：Senparc - 20
    
    修改标识：Senparc - 20230716
    修改描述：v2.3.1.1 创建默认 NeuChar 配置根目录创建

----------------------------------------------------------------*/


using Senparc.CO2NET.Extensions;
using Senparc.CO2NET.Helpers;
using Senparc.NeuChar.Helpers;
using Senparc.NeuChar.NeuralSystems;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Senparc.NeuChar
{
    /// <summary>
    /// 神经系统，整个系统数据的根节点
    /// </summary>
    public class NeuralSystem
    {
        #region 单例

        //静态SearchCache
        public static NeuralSystem Instance
        {
            get
            {
                return NeuralSystemNested.instance;//返回Nested类中的静态成员instance
            }
        }

        class NeuralSystemNested
        {
            static NeuralSystemNested()
            {
            }
            //将instance设为一个初始化的BaseCacheStrategy新实例
            internal static readonly NeuralSystem instance = new NeuralSystem();
        }

        #endregion

        //TODO：开发流程：实体->JSON/XML->General

        /// <summary>
        /// 默认配置文件内容
        /// </summary>
        internal const string DEFAULT_CONFIG_FILE_CONENT = "{}";
        internal const string CHECK_CNNECTION_RESULT = "OK";
        internal const string DEFAULT_MULIT_TENANT_ID = "_Default";

        /// <summary>
        /// NeuChar 域名
        /// </summary>
        public string NeuCharDomainName { get; set; } = "https://www.neuchar.com";

        /// <summary>
        /// 根节点（Key：MultiTenantId）
        /// </summary>
        public ConcurrentDictionary<string, INeuralNode> RootCollection { get; set; } = new ConcurrentDictionary<string, INeuralNode>();

        /// <summary>
        /// NeuChar 核心神经系统，包含所有神经节点信息
        /// </summary>
        NeuralSystem()
        {
            //获取所有配置并初始化

            //TODO:当配置文件多了之后可以遍历所有的配置文件

            //TODO:解密

            ReloadNode(null);
        }

        /// <summary>
        /// 初始化 Root 参数
        /// </summary>
        private void InitRoot(string multiTenantId)
        {
            multiTenantId = NeuCharConfigHelper.TryGetDefaultMultiTenantId(multiTenantId);

            //ConcurrentDictionary<string, INeuralNode> rootCollection = new ConcurrentDictionary<string, INeuralNode>();
            ////INeuralNode root = new RootNeuralNode();
            //RootCollection = rootCollection;

            RootCollection[multiTenantId] = new RootNeuralNode();//强制清空

            //TODO: 从文件中获取所有对象
        }

        /// <summary>
        /// 加载节点信息
        /// </summary>
        public void ReloadNode(string multiTenantId)
        {
            if (multiTenantId == null)
            {
                //全部重新载入
                var rootPath = NeuCharConfigHelper.GetNeuCharRootConfigBasePath();
                FileHelper.TryCreateDirectory(rootPath);

                var tenants = Directory.GetDirectories(rootPath).Select(Path.GetFileName);
                foreach (var tenant in tenants)
                {
                    ReloadNode(tenant);
                }
            }

            multiTenantId = NeuCharConfigHelper.TryGetDefaultMultiTenantId(multiTenantId);

            InitRoot(multiTenantId);//独立放在外面强制执行

            var path = NeuCharConfigHelper.GetNeuCharRootConfigFilePath(multiTenantId);

            var file = Path.Combine(path, "NeuCharRoot.config");
            //SenparcTrace.SendCustomLog("NeuChar file path", file);

            if (File.Exists(file))
            {
                using (var fs = new FileStream(file, FileMode.Open))
                {
                    using (var sr = new StreamReader(fs))
                    {
                        var configRootJson = sr.ReadToEnd();
                        //TODO:可以进行格式和版本校验

                        //SenparcTrace.SendCustomLog("NeuChar Saved ConfigRoot", configRootJson);

                        var configRoot = SerializerHelper.GetObject<ConfigRoot>(configRootJson);
                        //SenparcTrace.SendCustomLog("NeuChar NeuralSystem", configRoot.ToJson());
                        if (configRoot != null && configRoot.Configs != null)
                        {
                            object[] connfigs = configRoot.Configs;//SerializerHelper.GetObject<List<BaseNeuralNode>>(configRoot.Configs);
                                                                   //SenparcTrace.SendCustomLog("NeuChar configs", connfigs.ToJson());

                            //转换成 List<BaseNeuralNode> 对象
                            var configsJsonString = connfigs.ToJson();
                            var configsList = SerializerHelper.GetObject<List<BaseNeuralNode>>(configsJsonString);

                            for (int i = 0; i < connfigs.Length; i++)
                            {
                                if (Senparc.NeuChar.Register.NeuralNodeRegisterCollection.ContainsKey(configsList[i].Name))
                                {
                                    var configNodeType = Senparc.NeuChar.Register.NeuralNodeRegisterCollection[configsList[i].Name];
                                    //SenparcTrace.SendCustomLog("NeuChar config type", configNodeType.FullName);

                                    var configNodeJsonStr = connfigs[i].ToJson();//转成字符串，方便再次反序列化到具体类型中

                                    var finalNeuralNode = Newtonsoft.Json.JsonConvert.DeserializeObject(configNodeJsonStr, configNodeType) as INeuralNode;

                                    //SenparcTrace.SendCustomLog("NeuChar finalNeuralNode", finalNeuralNode.ToJson());

                                    RootCollection[multiTenantId].SetChildNode(finalNeuralNode);

                                    //SerializerHelper.GetObject()
                                }
                            }
                        }

                        //foreach (var config in connfigs)
                        //{
                        //    SenparcTrace.SendCustomLog("NeuChar config", config.ToJson());
                        //    if (Senparc.NeuChar.Register.NeuralNodeRegisterCollection.ContainsKey(config.Name))
                        //    {
                        //        var configNodeType = Senparc.NeuChar.Register.NeuralNodeRegisterCollection[config.Name];
                        //        SenparcTrace.SendCustomLog("NeuChar config type", configNodeType.FullName);

                        //    }

                        //}

                    }
                }
            }
        }

        ///// <summary>
        ///// 尝试获取合法的 multiTenantId，如果为空，则返回默认值
        ///// </summary>
        ///// <param name="multiTenantId"></param>
        ///// <returns></returns>
        //private string TryGetDefaultMultiTenantId(string multiTenantId)
        //{
        //    if (multiTenantId.IsNullOrEmpty())
        //    {
        //        //没有提供 MultiTenantId，在使用默认目录
        //        multiTenantId = DEFAULT_MULIT_TENANT_ID;
        //        if (!RootCollection.ContainsKey(multiTenantId))
        //        {
        //            RootCollection[multiTenantId] = new RootNeuralNode();//初始化
        //        }
        //        return multiTenantId;
        //    }

        //    return multiTenantId;
        //}

        /// <summary>
        /// 获取指定Name的节点
        /// <para>TODO：建立索引搜索</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parentNode">父节点</param>
        /// <returns></returns>
        public INeuralNode GetNode(string name, string multiTenantId, INeuralNode parentNode = null)
        {
            multiTenantId = NeuCharConfigHelper.TryGetDefaultMultiTenantId(multiTenantId);
            if (!RootCollection.ContainsKey(multiTenantId))
            {
                RootCollection[multiTenantId] = new RootNeuralNode();//初始化
            }

            if (parentNode == null)
            {
                parentNode = RootCollection[multiTenantId];
            }

            INeuralNode foundNode = null;

            if (parentNode.Name == name)
            {
                foundNode = parentNode;
            }

            if (foundNode == null && parentNode.ChildrenNodes != null && parentNode.ChildrenNodes.Count > 0)
            {
                foreach (var node in parentNode.ChildrenNodes)
                {

                    foundNode = GetNode(name, multiTenantId, node);//监测当前节点
                    if (foundNode != null)
                    {
                        break;
                    }
                }
            }

            return foundNode;
        }
    }
}
