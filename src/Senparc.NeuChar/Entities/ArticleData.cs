using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.Entities
{
    /// <summary>
    /// 图文数据
    /// </summary>
    public class ArticleData
    {
        /// <summary>
        /// 封面图地址
        /// </summary>
        public string ThumbCoverUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 作者
        /// </summary>
        public string Author
        {
            get;
            set;
        }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content
        {
            get;
            set;
        }

        /// <summary>
        /// 摘要
        /// </summary>
        public string Digest
        {
            get;
            set;
        }

        /// <summary>
        /// 原地址
        /// </summary>
        public string ContentSourceUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 多图文
        /// </summary>
        public List<string> ArticleIds
        {
            get;
            set;
        }

        public ArticleData()
        {
            ArticleIds = new List<string>();
        }
    }
}
