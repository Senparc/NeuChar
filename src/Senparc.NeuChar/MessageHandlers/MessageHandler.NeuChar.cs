using Senparc.CO2NET.Extensions;
using Senparc.CO2NET.Helpers;
using Senparc.CO2NET.Trace;
using Senparc.CO2NET.Utilities;
using Senparc.NeuChar.Entities;
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
                var file = Path.Combine(path, "NeuCharRoot.config");
                string result = null;

                if (!File.Exists(file))
                {
                    using (var fs = new FileStream(file, FileMode.CreateNew))
                    {
                        using (var sw = new StreamWriter(fs))
                        {
                            sw.WriteLine("{}");
                        }
                        fs.Flush();
                    }
                }

                switch (requestMessage.NeuCharMessageType)
                {
                    case NeuCharActionType.GetConfig:
                        {
                            if (File.Exists(file))
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
                                result = "{}";//TODO:初始化一个对象
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

                            var fileTemp = Path.Combine(path, $"NeuCharRoot.temp.{DateTime.Now.ToString("yyyyMMdd-HHmmss")}.config");
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
                            File.Move(file, file.Replace(".config", $".bak.{DateTime.Now.ToString("yyyyMMdd-HHmmss")}.config"));
                            File.Move(fileTemp, file);

                            //刷新数据
                            var neuralSystem = NeuralSystem.Instance;
                            neuralSystem.ReloadNode();
                        }
                        break;
                    default:
                        break;
                }


                var successMsg = new
                {
                    success = true,
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
