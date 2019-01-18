using System;
using System.Collections.Generic;

namespace Senparc.NeuChar.Entities.App
{
    /// <summary>
    /// App 输入输出实体
    /// </summary>
    public class ConfigParamModel
    {
        /// <summary>
        /// 输入参数
        /// </summary>
        public List<InputParam> InputParams { get; set; }
        /// <summary>
        /// 输出参数
        /// </summary>
        public List<OutputParam> OutputParams { get; set; }
        /// <summary>
        /// 是否强制为自动开始
        /// </summary>
        public bool ForceAutoStart { get; set; }
        /// <summary>
        /// 是否强制为隐藏流程
        /// </summary>
        public bool ForceHide { get; set; }
        /// <summary>
        /// 后台管理 Url
        /// <para>如：https://neuchar.weiweihi.com/User/QuestionActivity?app={appId}</para>
        /// <para>appId 是 InputParams 中 Name 为 appId 的值进行替换</para>
        /// </summary>
        public string AdminUrl { get; set; }
    }

    /// <summary>
    /// App 参数基类
    /// </summary>
    [Serializable]
    public class BaseParam
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 编号
        /// </summary>
        public Guid ParamId { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Presentation { get; set; }
    }

    /// <summary>
    /// App 输入参数
    /// </summary>
    [Serializable]
    public class InputParam : BaseParam
    {
        /// <summary>
        /// 必填参数
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// 输入内容[不需要提供]
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// App提供选项
        /// </summary>
        public List<string> OptionValues { get; set; } = new List<string>();

        /// <summary>
        /// 自动填充数据接口地址
        /// <para>如：https://neuchar.weiweihi.com/User/AutoFillWindow?app={appId}</para>
        /// <para>appId 是 InputParams 中 Name 为 appId 的值进行替换</para>
        /// </summary>
        public string AutoFillUrl { get; set; }
    }

    /// <summary>
    /// App 输出参数
    /// </summary>
    [Serializable]
    public class OutputParam : BaseParam
    {
        /// <summary>
        /// 输出内容
        /// </summary>
        public string Value { get; set; }
    }
}
