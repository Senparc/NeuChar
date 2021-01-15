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
    
    文件名：NeuCharAppController.cs
    文件功能描述：用于快速实现 NeuChar 开发者发布的 App 与 NeuChar 平台交互（如状态发送）的默认 Controller
                  如果你有已经自己实现的带有 MessageHandler 的 Controller，也可以不使用这个基类
    
    
    创建标识：Senparc - 20200105

    -- 从 Senparc.NeuChar.APP 移植--

----------------------------------------------------------------*/

using Senparc.NeuChar.App.MessageHandlers;
using Senparc.NeuChar.MessageHandlers.CheckSignatures;
using System;
using Senparc.CO2NET.Trace;
using System.IO;
using Senparc.NeuChar.App.Entities;
using Senparc.CO2NET.AspNet.HttpUtility;
#if NET45
using System.Web.Mvc;
#else
using Microsoft.AspNetCore.Mvc;
#endif

namespace Senparc.NeuChar.App.Controllers
{
    /// <summary>
    /// 用于快速实现 NeuChar 开发者发布的 App 与 NeuChar 平台交互（如状态发送）的默认 Controller 
    /// <para>如果你有已经自己实现的带有 MessageHandler 的 Controller，也可以不使用这个基类</para>
    /// </summary>
    public abstract class NeuCharAppController : Controller
    {
        protected abstract string Token { get; }

        private readonly Func<string> _getRandomFileName = () => SystemTime.Now.ToString("yyyyMMdd-HHmmss") + Guid.NewGuid().ToString("n").Substring(0, 6);

        /// <summary>
        /// 后台验证地址（使用Get），微信后台的“接口配置信息”的Url填写如：http://sdk.weixin.senparc.com/weixin
        /// </summary>
        [HttpGet]
        [ActionName("NeuCharApp")]
        public virtual ActionResult Get(NeuCharAppPostModel postModel, string echostr, string neucharAppId)
        {
            postModel.Token = Token;
            postModel.AppId = neucharAppId;//加密暂时用不到
            if (postModel.Signature == CheckSignatureWeChat.GetSignature(postModel))
            {
                return Content(echostr); //返回随机字符串则表示验证通过
            }
            else
            {
                return Content($"failed:{postModel.Signature},{CheckSignatureWeChat.GetSignature(postModel.Timestamp, postModel.Nonce, Token)}。" +
                    $"如果你在浏览器中看到这句话，说明此地址可以被作为 NeuChar - App后台的Url，请注意保持Token一致。");
            }
        }


        /// <summary>
        /// 最简化的处理流程（不加密）
        /// </summary>
        [HttpPost]
        [ActionName("NeuCharApp")]
        public virtual ActionResult Post(NeuCharAppPostModel postModel, string neucharAppId)
        {
            postModel.Token = Token;
            postModel.AppId = neucharAppId;// $"NeuCharApp:AppId:{neucharAppId}";

            if (postModel.Signature != CheckSignatureWeChat.GetSignature(postModel))
            {
                return Content("参数错误！");
            }

            //postModel.EncodingAESKey = EncodingAESKey;//根据自己后台的设置保持一致
            //postModel.AppId = AppId;//根据自己后台的设置保持一致

            NeuCharAppMessageHandler messageHandler = null;
            try
            {
#if NET45
                messageHandler = new NeuCharAppMessageHandler(Request.InputStream, postModel);
#else
                messageHandler = new NeuCharAppMessageHandler(Request.GetRequestMemoryStream(), postModel);
#endif

                messageHandler.SaveRequestMessageLog();//记录 Request 日志（可选）

                messageHandler.Execute();//执行微信处理过程（关键）

                messageHandler.SaveResponseMessageLog();//记录 Response 日志（可选）

                var responseText = messageHandler.TextResponseMessage ?? "";
                return Content(responseText);

            }
            catch (Exception ex)
            {
                #region 异常处理
                SenparcTrace.Log($"NeuCharAppMessageHandler错误：{ex.Message}");

                var logPath = Path.Combine(messageHandler.GetLogPath(), $"Error_{_getRandomFileName()}.txt");
                using (TextWriter tw = new StreamWriter(logPath))
                {
                    tw.WriteLine("ExecptionMessage:" + ex.Message);
                    tw.WriteLine(ex.Source);
                    tw.WriteLine(ex.StackTrace);
                    //tw.WriteLine("InnerExecptionMessage:" + ex.InnerException.Message);

                    if (messageHandler.ResponseDocument != null)
                    {
                        tw.WriteLine(messageHandler.ResponseDocument.ToString());
                    }

                    if (ex.InnerException != null)
                    {
                        tw.WriteLine("========= InnerException =========");
                        tw.WriteLine(ex.InnerException.Message);
                        tw.WriteLine(ex.InnerException.Source);
                        tw.WriteLine(ex.InnerException.StackTrace);
                    }

                    tw.Flush();
                    tw.Close();
                }
                return Content("");
                #endregion
            }
        }
    }
}
