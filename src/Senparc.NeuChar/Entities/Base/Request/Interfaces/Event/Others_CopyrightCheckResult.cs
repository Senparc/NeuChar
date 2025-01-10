using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.Entities
{

    public class CopyrightCheckResult
    {
        /// <summary>
        /// 消息数量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 各个单图文校验结果
        /// </summary>
        public List<CopyrightCheckResult_ResultList> ResultList { get; set; }
        /// <summary>
        /// 整体校验结果 1-未被判为转载，可以群发，2-被判为转载，可以群发，3-被判为转载，不能群发
        /// </summary>
        public int CheckState { get; set; }

        public CopyrightCheckResult()
        {
            ResultList = new List<CopyrightCheckResult_ResultList>();
        }
    }


    /// <summary>
    /// 单图文校验结果
    /// </summary>
    public class CopyrightCheckResult_ResultList
    {
        public CopyrightCheckResult_ResultList_Item item { get; set; }

        public CopyrightCheckResult_ResultList()
        {
            item = new CopyrightCheckResult_ResultList_Item();
        }
    }

    /// <summary>
    /// 单图文校验结果
    /// </summary>
    public class CopyrightCheckResult_ResultList_Item
    {
        /// <summary>
        /// 群发文章的序号，从1开始
        /// </summary>
        public int ArticleIdx { get; set; }
        /// <summary>
        /// 用户声明文章的状态
        /// </summary>
        public int UserDeclareState { get; set; }
        /// <summary>
        /// 系统校验的状态
        /// </summary>
        public int AuditState { get; set; }
        /// <summary>
        /// 相似原创文的url
        /// </summary>
        public string OriginalArticleUrl { get; set; }
        /// <summary>
        /// 相似原创文的类型
        /// </summary>
        public int OriginalArticleType { get; set; }
        /// <summary>
        /// 是否能转载
        /// </summary>
        public int CanReprint { get; set; }
        /// <summary>
        /// 是否需要替换成原创文内容
        /// </summary>
        public int NeedReplaceContent { get; set; }
        /// <summary>
        /// 是否需要注明转载来源
        /// </summary>
        public int NeedShowReprintSource { get; set; }
    }
}
