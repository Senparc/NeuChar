using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.Entities
{
    /// <summary>
    /// 所有ResponseMessageNews的接口
    /// </summary>
    public interface IResponseMessageNews : IResponseMessageBase
    {
        /// <summary>
        /// 文章数量
        /// </summary>
        int ArticleCount { get; set; }//这里开放set只为了逆向从Response的Xml转成实体的操作一致性，没有实际意义。

        /// <summary>
        /// 文章列表，微信客户端只能输出前10条（可能未来数字会有变化，出于视觉效果考虑，建议控制在8条以内）
        /// </summary>
        List<Article> Articles { get; set; }
    }
}
