/*----------------------------------------------------------------
    Copyright (C) 2021 Senparc
    
    文件名：ContactSyncToken.cs
    文件功能描述：企业微信服务商推广码注册成功返回的通讯录迁移的凭证信息。仅当注册推广包开启通讯录迁移接口时返回该参数
    
    
    创建标识：Billzjh - 20201210
    
----------------------------------------------------------------*/

namespace Senparc.NeuChar.Entities
{
    /// <summary>
    /// 企业微信服务商推广码注册成功返回的通讯录迁移的凭证信息。仅当注册推广包开启通讯录迁移接口时返回该参数
    /// </summary>
    public class ContactSyncToken
    {
        /// <summary>
        /// 通讯录api接口调用凭证，有全部通讯录读写权限
        /// </summary>
        public string AccessToken { get; set; }
        /// <summary>
        /// AccessToken的有效时间（秒）
        /// </summary>
        public int ExpiresIn { get; set; }
    }

    /// <summary>
    /// 企业微信服务商推广码注册成功返回的 授权管理员的信息
    /// </summary>
    public class AuthUserInfoModel
    {
        /// <summary>
        /// 授权管理员的userid
        /// </summary>
        public string UserId { get; set; }
    }
}
