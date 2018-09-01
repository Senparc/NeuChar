using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar
{
    public enum DataType
    {
        List,
        Unique
    }

    public enum ApiType
    {
        AccessToken,
        Normal
    }

    /// <summary>
    /// NeuChar 消息的乐行
    /// </summary>
    public enum NeuCharMessageType
    {
        /// <summary>
        /// 获取配置
        /// </summary>
        GetConfig,
        /// <summary>
        /// 储存配置
        /// </summary>
        SaveConfig,
    }


    /// <summary>
    /// AppStore状态
    /// </summary>
    public enum AppStoreState
    {
        /// <summary>
        /// 无状态
        /// </summary>
        None = 1,
        /// <summary>
        /// 已进入应用状态
        /// </summary>
        Enter = 2,
        /// <summary>
        /// 退出App状态（临时传输状态，退出后即为None）
        /// </summary>
        Exit = 4
    }
}
