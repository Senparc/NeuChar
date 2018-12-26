using Senparc.CO2NET.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.NeuralSystems
{
    /// <summary>
    /// 单项 App 订阅信息
    /// </summary>
    public class AppDataItem : IConfigItem
    {
        #region IConfigItem
        /// <summary>
        /// 备注名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 唯一编号
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 版本号，如："2018.9.27.1"
        /// </summary>
        public string Version { get; set; }
        #endregion

        /// <summary>
        /// 过期时间（Unix时间）
        /// </summary>
        public long ExpireTime { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTimeOffset ExpireDateTime { get { return DateTimeHelper.GetDateTimeFromXml(ExpireTime); } }

        /// <summary>
        /// 进入对话状态关键词
        /// </summary>
        public string MessageEnterWord { get; set; }
        /// <summary>
        /// 退出对话关键词
        /// </summary>
        public string MessageExitWord { get; set; }
        /// <summary>
        /// 消息关键词
        /// </summary>
        public string MessageKeywords { get; set; }
        /// <summary>
        /// 状态保持分钟数（0为无状态）
        /// </summary>
        public int MessageKeepTime { get; set; }

    }
}
