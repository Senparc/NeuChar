using Senparc.CO2NET.APM;
using Senparc.CO2NET.Extensions;
using Senparc.CO2NET.Helpers;
using Senparc.CO2NET.Trace;
using Senparc.CO2NET.Utilities;
using Senparc.NeuChar.Entities;
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

                            File.Delete(fileTemp);

                            using (var fs = new FileStream(fileTemp, FileMode.CreateNew))
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

                            if (configRoot== null || configRoot.Domain.IsNullOrWhiteSpace())
                            {
                                success = false;
                                result = "未指定 Domain!";
                                break;
                            }

                            var co2netDataOperation = new DataOperation(configRoot.Domain);

                            //获取所有数据
                            var dataItems = co2netDataOperation.ReadAndCleanDataItems(configRoot.RemoveData);
                            result = dataItems.ToString();
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
