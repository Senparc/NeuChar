#region Apache License Version 2.0
/*----------------------------------------------------------------

Copyright 2018 Suzhou Senparc Network Technology Co.,Ltd.

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
    Copyright (C) 2018 Senparc
    
    文件名：MessageHandler.NeuChar.cs
    文件功能描述：微信请求中有关 NeuChar 方法的集中处理方法
    
    
    创建标识：Senparc - 20180910
    
    修改标识：Senparc - 20190106
    修改描述：v0.6.0 添加 PushNeuCharAppConfig 和 PullNeuCharAppConfig 消息类型

----------------------------------------------------------------*/

using Senparc.CO2NET.APM;
using Senparc.CO2NET.Extensions;
using Senparc.CO2NET.Helpers;
using Senparc.CO2NET.Trace;
using Senparc.CO2NET.Utilities;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Entities.App;
using Senparc.NeuChar.NeuralSystems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.MessageHandlers
{
    public abstract partial class MessageHandler<TC, TRequest, TResponse>
    {
        static MessageHandler()
        {
            //注册节点类型
            Senparc.NeuChar.Register.RegisterNeuralNode("MessageHandlerNode", typeof(MessageHandlerNode));
            Senparc.NeuChar.Register.RegisterNeuralNode("AppDataNode", typeof(AppDataNode));
        }

        #region NeuChar 方法

        /// <summary>
        /// NeuChar 请求
        /// </summary>
        public virtual IResponseMessageBase OnNeuCharRequest(RequestMessageNeuChar requestMessage)
        {
            try
            {
                var path = ServerUtility.ContentRootMapPath("~/App_Data/NeuChar");
                //SenparcTrace.SendCustomLog("OnNeuCharRequest path", path);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var file = Path.Combine(path, "NeuCharRoot.config");
                bool success = true;
                string result = null;

                var configFileExisted = File.Exists(file);
                if (!configFileExisted)
                {
                    using (var fs = new FileStream(file, FileMode.CreateNew))
                    {
                        using (var sw = new StreamWriter(fs))
                        {
                            sw.WriteLine(NeuralSystem.DEFAULT_CONFIG_FILE_CONENT);
                        }
                        fs.Flush();
                    }
                }

                switch (requestMessage.NeuCharMessageType)
                {
                    case NeuCharActionType.GetConfig:
                        {
                            if (configFileExisted)//本次对话会创建，但不在读取，利可读取可能会发生“无法访问已关闭文件”的错误
                            {
                                using (var fs = FileHelper.GetFileStream(file))
                                {
                                    using (var sr = new StreamReader(fs, Encoding.UTF8))
                                    {
                                        var json = sr.ReadToEnd();
                                        result = json;
                                    }
                                }
                            }
                            else
                            {
                                result = NeuralSystem.DEFAULT_CONFIG_FILE_CONENT;//TODO:初始化一个对象
                            }
                        }
                        break;
                    case NeuCharActionType.SaveConfig:
                        {
                            var configRootJson = requestMessage.ConfigRoot;
                            SenparcTrace.SendCustomLog("收到NeuCharRequest", "字符串长度：" + configRootJson.Length.ToString());
                            var configRoot = SerializerHelper.GetObject<ConfigRoot>(configRootJson);//这里只做序列化校验

                            //TODO:进行验证


                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }

                            var fileTemp = Path.Combine(path, $"NeuCharRoot.temp.{SystemTime.Now.ToString("yyyyMMdd-HHmmss")}.config");
                            //TODO：后期也可以考虑把不同模块分离到不同的文件中

                            using (var fs = new FileStream(fileTemp, FileMode.Create))
                            {
                                using (var sw = new StreamWriter(fs))
                                {
                                    sw.Write(configRootJson);
                                    sw.Flush();
                                }
                            }

                            //历史文件备份，并替换临时文件
                            File.Move(file, file.Replace(".config", $".bak.{SystemTime.Now.ToString("yyyyMMdd-HHmmss")}.config"));
                            File.Move(fileTemp, file);

                            //刷新数据
                            var neuralSystem = NeuralSystem.Instance;
                            neuralSystem.ReloadNode();
                        }
                        break;
                    case NeuCharActionType.CheckNeuChar:
                        {
                            //TODO：进行有效性检验
                            var configRoot = requestMessage.ConfigRoot?.GetObject<APMDomainConfig>();

                            if (configRoot == null || configRoot.Domain.IsNullOrWhiteSpace())
                            {
                                success = false;
                                result = "未指定 Domain!";
                                break;
                            }

                            var co2netDataOperation = new DataOperation(configRoot.Domain);

                            //获取所有数据
                            var dataItems = co2netDataOperation.ReadAndCleanDataItems(configRoot.RemoveData, true);
                            result = dataItems.ToJson();
                        }
                        break;
                    case NeuCharActionType.PushNeuCharAppConfig:
                        {
                            var configFileDir = Path.Combine(path, "AppConfig");
                            if (!Directory.Exists(configFileDir))
                            {
                                Directory.CreateDirectory(configFileDir);//这里也可以不创建，除非是为了推送
                            }

                            //还原一次，为了统一格式，并未后续处理提供能力（例如调整缩进格式）
                            var requestData = requestMessage.RequestData.GetObject<PushConfigRequestData>();
                            var mainVersion = requestData.Version.Split('.')[0];//主版本号
                            //配置文件路径：~/App_Data/NeuChar/AppConfig/123-v1.config
                            var configFilePath = Path.Combine(configFileDir, $"{requestData.AppId}-v{mainVersion}.config");

                            using (var fs = new FileStream(configFilePath, FileMode.Create))
                            {
                                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                                {
                                    var json = requestData.Config.ToJson(true);//带缩进格式的 JSON 字符串
                                    sw.Write(json);//写入 Json 文件
                                    sw.Flush();
                                }
                            }
                            result = "OK";
                        }
                        break;
                    case NeuCharActionType.PullNeuCharAppConfig:
                        {
                            var requestData = requestMessage.RequestData.GetObject<PullConfigRequestData>();
                            var mainVersion = requestData.Version.Split('.')[0];//主版本号

                            var configFileDir = Path.Combine(path, "AppConfig");
                            //配置文件路径：~/App_Data/NeuChar/AppConfig/123-v1.config
                            var configFilePath = Path.Combine(configFileDir, $"{requestData.AppId}-v{mainVersion}.config");
                            if (!File.Exists(configFilePath))
                            {
                                //文件不存在
                                result = $"配置文件不存在，请先推送或设置配置文件，地址：{configFilePath}";
                                success = false;
                            }
                            else
                            {
                                //读取内容
                                using (var fs = FileHelper.GetFileStream(configFilePath))
                                {
                                    using (var sr = new StreamReader(fs, Encoding.UTF8))
                                    {
                                        var json = sr.ReadToEnd();//带缩进格式的 JSON 字符串（文件中的原样）
                                        result = json;
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }

                var successMsg = new
                {
                    success = success,
                    result = result
                };
                TextResponseMessage = successMsg.ToJson();
            }
            catch (Exception ex)
            {
                var errMsg = new
                {
                    success = false,
                    result = ex.Message
                };
                TextResponseMessage = errMsg.ToJson();
            }

            return null;
        }

        #endregion

    }
}
