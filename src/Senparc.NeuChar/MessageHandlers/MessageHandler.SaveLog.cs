using Senparc.CO2NET.APM;
using Senparc.CO2NET.Exceptions;
using Senparc.CO2NET.Extensions;
using Senparc.CO2NET.Helpers;
using Senparc.CO2NET.MessageQueue;
using Senparc.CO2NET.Trace;
using Senparc.CO2NET.Utilities;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Exceptions;
using Senparc.NeuChar.NeuralSystems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
#if NET35 || NET40 || NET45
using System.Web;
#endif
namespace Senparc.NeuChar.MessageHandlers
{
    public abstract partial class MessageHandler<TC, TRequest, TResponse>
    {
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
        /// <param name="logPath">保存日志目录，默认为 ~/App_Data/&lt;模块类型&gt;/<yyyy-MM-dd>/</param>
        public void SaveRequestMessageLog(string logPath = null)
        {
            try
            {
                logPath = logPath ?? GetLogPath();
                SenparcMessageQueue queue = new SenparcMessageQueue();
                var key = Guid.NewGuid().ToString();
                queue.Add(key, () =>
                {
                    this.RequestDocument.Save(Path.Combine(logPath, string.Format("{0}_Request_{1}_{2}.txt", _getRandomFileName(),
                                              this.RequestMessage.FromUserName,
                                              this.RequestMessage.MsgType)));
                    if (this.UsingEcryptMessage && this.EcryptRequestDocument != null)
                    {
                        this.EcryptRequestDocument.Save(Path.Combine(logPath, string.Format("{0}_Request_Ecrypt_{1}_{2}.txt", _getRandomFileName(),
                            this.RequestMessage.FromUserName,
                            this.RequestMessage.MsgType)));
                    }
                });
            }
            catch (Exception ex)
            {
                new MessageHandlerException(ex.Message, ex);
            }
        }

        /// <summary>
        /// 保存响应信息
        /// <para>测试时可开启此记录，帮助跟踪数据，使用前请确保App_Data文件夹存在，且有读写权限。</para>
        /// </summary>
        /// <param name="logPath">保存日志目录，默认为 ~/App_Data/&lt;模块类型&gt;/<yyyy-MM-dd>/</param>
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
                            this.ResponseMessage.ToUserName,
                            this.ResponseMessage.MsgType)));
                    }

                    if (this.UsingEcryptMessage &&
                        this.FinalResponseDocument != null && this.FinalResponseDocument.Root != null)
                    {
                        //记录加密后的响应信息
                        this.FinalResponseDocument.Save(Path.Combine(logPath, string.Format("{0}_Response_Final_{1}_{2}.txt", _getRandomFileName(),
                            this.ResponseMessage.ToUserName,
                            this.ResponseMessage.MsgType)));
                    }
                });
            }
            catch (Exception ex)
            {
                new MessageHandlerException(ex.Message, ex);
            }
        }

        #endregion

    }
}
