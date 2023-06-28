using System.Collections.Generic;

namespace Senparc.NeuChar.Entities
{
    /// <summary>
    /// 文章详情
    /// </summary>
    public class Article_Detail
    {
        /// <summary>
        /// 当发布状态为0时（即成功）时，返回文章数量
        /// </summary>
        public int count { get; set; }

        /// <summary>
        /// 文章详情项
        /// </summary>
        public List<Article_Detail_Item> item { get; set; }
    }

    /// <summary>
    /// 文章详情项
    /// </summary>
    public class Article_Detail_Item
    {
        /// <summary>
        /// 当发布状态为0时（即成功）时，返回文章对应的编号
        /// </summary>
        public int idx { get; set; }
        /// <summary>
        /// 当发布状态为0时（即成功）时，返回图文的永久链接
        /// </summary>
        public string article_url { get; set; }
    }
}
