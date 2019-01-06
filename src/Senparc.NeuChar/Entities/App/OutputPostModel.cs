using System;
using System.Collections.Generic;

namespace Senparc.NeuChar.Entities.App
{
    /// <summary>
    /// App发送到应用中枢数据
    /// </summary>
    public class OutputPostModel
    {
        /// <summary>
        /// App 输出参数集合
        /// </summary>
        public List<OutputParam> PostData { get; set; }

        /// <summary>
        /// 用户OpenId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 步骤编号
        /// </summary>
        public Guid StepId { get; set; }

        /// <summary>
        /// 应用中枢编号
        /// </summary>
        public int WorkflowId { get; set; }

        /// <summary>
        /// App当前配置版本号
        /// </summary>
        public string AppVersion { get; set; }
    }
}
