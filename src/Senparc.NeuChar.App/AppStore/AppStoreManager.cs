#region Apache License Version 2.0
/*----------------------------------------------------------------

Copyright 2021 Jeffrey Su & Suzhou Senparc Network Technology Co.,Ltd.

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
  
    文件名：AppStoreManager.cs
    文件功能描述：AppStoreOAuth
    
    
    创建标识：Senparc - 20150319
----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using Senparc.NeuChar.App.AppStore.Api;
using Senparc.NeuChar.App.Exceptions;

namespace Senparc.NeuChar.App.AppStore
{
    /// <summary>
    /// AppStore 管理类
    /// </summary>
    public class AppStoreManager
    {
        /// <summary>
        /// 默认域名
        /// </summary>
        public const string DEFAULT_URL = "https://www.neuchar.com"; //"https://api.weiweihi.com:8080";//默认Api Url地址

        //private static string _appKey;
        //private static string _secret;
        private static PassportCollection _passportCollection;
        public static PassportCollection PassportCollection
        {
            get
            {
                if (_passportCollection == null)
                {
                    //LoadPassport();
                    _passportCollection = new PassportCollection();
                }
                return _passportCollection;
            }
            set { _passportCollection = value; }
        }

        /// <summary>
        /// 获取 PassportBag
        /// </summary>
        /// <param name="appKey"></param>
        /// <returns></returns>
        public static PassportBag GetPassportBag(string appKey)
        {
            if (PassportCollection.ContainsKey(appKey))
            {
                return PassportCollection[appKey];
            }
            return null;
        }

        /// <summary>
        /// API 默认路径（紧跟域名）
        /// </summary>
        public const string BasicApiPath = "/App/Api/";

        /// <summary>
        /// 注册P2P应用基本信息（可以选择不立即载入Passport以节省系统启动时间）
        /// </summary>
        /// <param name="appKey">P2P后台申请到微信应用后的AppKey</param>
        /// <param name="secret">AppKey对应的Secret</param>
        /// <param name="url">API地址，建议使用默认值</param>
        /// <param name="getPassportImmediately">是否马上获取Passport，默认为False</param>
        private static void Register(IServiceProvider serviceProvider, string appKey, string secret, string url = DEFAULT_URL, bool getPassportImmediately = false)
        {
            //if (PassportCollection.BasicUrl != url)
            //{
            ////不使用默认地址
            PassportCollection.BasicUrl = url + BasicApiPath;
            //}

            PassportCollection[appKey] = new PassportBag(appKey, secret, url + BasicApiPath);
            if (getPassportImmediately)
            {
                ApplyPassport(serviceProvider, appKey, secret, url);
            }
        }

        /// <summary>
        /// 申请新的通行证。
        /// 每次调用完毕前将有1秒左右的Thread.Sleep时间
        /// </summary>
        public static void ApplyPassport(IServiceProvider serviceProvider, string appKey, string appSecret, string url = DEFAULT_URL)
        {
            if (!PassportCollection.ContainsKey(appKey))
            {
                //如果不存在，则自动注册（注册之后PassportCollection[appKey]一定有存在值）
                Register(serviceProvider, appKey, appSecret, url, true);
            }

            var passportBag = PassportCollection[appKey];

            var getPassportUrl = PassportCollection.BasicUrl + "GetPassport";
            var formData = new Dictionary<string, string>();
            formData["appKey"] = passportBag.AppKey;
            formData["secret"] = passportBag.AppSecret;
            var result = CO2NET.HttpUtility.Post.PostGetJson<PassportResult>(serviceProvider, getPassportUrl, formData: formData);
            if (result.Result != AppResultKind.成功)
            {
                throw new NeuCharAppException("获取Passort失败！错误信息：" + result.Result, null);
            }

            passportBag.Passport = result.Data;
            passportBag.Passport.ApiUrl = PassportCollection.BasicUrl;
        }

        /// <summary>
        /// 清除当前的通行证
        /// </summary>
        public static void RemovePassport(string appKey)
        {
            try
            {
                PassportCollection.Remove(appKey);
            }
            catch
            {
            }
        }

        /// <summary>
        /// 获取appKey对应的接口集合。
        /// 调用此方法请确认此appKey已经成功使用SdkManager.Register(appKey, appSecret, appUrl)方法注册过。
        /// </summary>
        /// <param name="appKey"></param>
        /// <returns></returns>
        public static ApiContainer GetApiContainer(IServiceProvider serviceProvider, string appKey, string appSecret, string url = DEFAULT_URL)
        {
            return new ApiContainer(serviceProvider, appKey, appSecret, url);
        }
    }
}
