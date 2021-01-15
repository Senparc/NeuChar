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
    
    文件名：MessageHandler.SaveLog.cs
    文件功能描述：微信请求日志记录
    
    
    创建标识：Senparc - 20181108
    
    修改标识：Senparc - 20190912
    修改描述：v0.7.6 优化 MessageHandler.SaveResponseMessageLog() 方法

----------------------------------------------------------------*/

using Senparc.CO2NET.MessageQueue;
using Senparc.NeuChar.Context;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Exceptions;
using System;
using System.IO;
#if NET35 || NET40 || NET45
using System.Web;
#endif
namespace Senparc.NeuChar.MessageHandlers
{
    public abstract partial class MessageHandler<TMC, TRequest, TResponse>
        where TMC : class, IMessageContext<TRequest, TResponse>, new()
        where TRequest : class, IRequestMessageBase
        where TResponse : class, IResponseMessageBase
    {
        /// <summary>
        /// 随机日志文件名
        /// </summary>
        readonly Func<string> _getRandomFileName = () => SystemTime.Now.ToString("yyyyMMdd-HHmmss") + Guid.NewGuid().ToString("n").Substring(0, 6);

        #region 记录 日志 

        /// <summary>
        /// 获取日志保存地址
        /// </summary>
        /// <returns></returns>
        public string GetLogPath()
        {
            //#if NET35 || NET40 || NET45
            //            var appDomainAppPath = HttpRuntime.AppDomainAppPath;
            //#else
            //            var appDomainAppPath = Senparc.CO2NET.Config.RootDictionaryPath; //dll所在目录：;
            //#endif

            var logPath = CO2NET.Utilities.ServerUtility.ContentRootMapPath($"~/App_Data/{this.MessageEntityEnlightener?.PlatformType.ToString()}/{ SystemTime.Now.ToString("yyyy-MM-dd")}/");
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }

            return logPath;
        }

        /// <summary>
        /// 保存请求信息
        /// <para>测试时可开启此记录，帮助跟踪数据，使用前请确保App_Data文件夹存在，且有读写权限。</para>
        /// </summary>
        /// <param name="logPath">保存日志目录，默认为 ~/App_Data/&lt;模块类型&gt;/&lt;yyyy-MM-dd&gt;/</param>
        public void SaveRequestMessageLog(string logPath = null)
        {
            try
            {
                logPath = logPath ?? GetLogPath();
                SenparcMessageQueue queue = new SenparcMessageQueue();
                var key = Guid.NewGuid().ToString();
                queue.Add(key, () =>
                {
                    if (this.RequestDocument != null)
                    {
                        this.RequestDocument.Save(Path.Combine(logPath, string.Format("{0}_Request_{1}_{2}.txt", _getRandomFileName(),
                                              this.RequestMessage?.FromUserName,
                                              this.RequestMessage?.MsgType)));
                    }
                    else
                    {
                        System.IO.File.WriteAllText(Path.Combine(logPath, string.Format("{0}_UntreatedRequest.txt", _getRandomFileName())),
                                              this.TextResponseMessage);
                    }

                    if (this.UsingEncryptMessage && this.EcryptRequestDocument != null)
                    {
                        this.EcryptRequestDocument.Save(Path.Combine(logPath, string.Format("{0}_Request_Ecrypt_{1}_{2}.txt", _getRandomFileName(),
                            this.RequestMessage.FromUserName,
                            this.RequestMessage.MsgType)));
                    }
                });
            }
            catch (Exception ex)
            {
                _ = new MessageHandlerException(ex.Message, ex);
            }
        }

        /// <summary>
        /// 保存响应信息
        /// <para>测试时可开启此记录，帮助跟踪数据，使用前请确保App_Data文件夹存在，且有读写权限。</para>
        /// </summary>
        /// <param name="logPath">保存日志目录，默认为 ~/App_Data/&lt;模块类型&gt;/&lt;yyyy-MM-dd&gt;/</param>
        public void SaveResponseMessageLog(string logPath = null)
        {
            try
            {
                logPath = logPath ?? GetLogPath();

                SenparcMessageQueue queue = new SenparcMessageQueue();
                var key = Guid.NewGuid().ToString();
                queue.Add(key, () =>
                {

                    if (this.ResponseDocument != null && this.ResponseDocument.Root != null)
                    {
                        this.ResponseDocument.Save(Path.Combine(logPath, string.Format("{0}_Response_{1}_{2}.txt", _getRandomFileName(),
                            this.ResponseMessage?.ToUserName,
                            this.ResponseMessage?.MsgType)));
                    }

                    if (this.UsingEncryptMessage &&
                        this.FinalResponseDocument != null && this.FinalResponseDocument.Root != null)
                    {
                        //记录加密后的响应信息
                        this.FinalResponseDocument.Save(Path.Combine(logPath, string.Format("{0}_Response_Final_{1}_{2}.txt", _getRandomFileName(),
                            this.ResponseMessage?.ToUserName,
                            this.ResponseMessage?.MsgType)));
                    }

                    if (this.ResponseDocument == null && this.TextResponseMessage != null)
                    {
                        System.IO.File.WriteAllText(Path.Combine(logPath, string.Format("{0}_TextResponse_{1}_{2}.txt", _getRandomFileName(),
                            this.RequestMessage?.ToUserName,
                            this.RequestMessage?.MsgType)), this.TextResponseMessage);
                    }
                });
            }
            catch (Exception ex)
            {
                _ = new MessageHandlerException(ex.Message, ex);
            }
        }

        #endregion

    }
}
