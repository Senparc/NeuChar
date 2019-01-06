using System;
using System.Collections.Generic;

namespace Senparc.NeuChar.Entities.App
{
    /// <summary>
    /// 应用中枢发送到App数据
    /// </summary>
    public class InputPostModel
    {
        /// <summary>
        /// App 输入参数集合
        /// </summary>
        public List<InputParam> PostData { get; set; }

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
