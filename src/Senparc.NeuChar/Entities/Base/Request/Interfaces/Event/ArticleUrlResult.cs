/*----------------------------------------------------------------
    Copyright (C) 2021 Senparc
    
    文件名：ArticleUrlResult.cs
    文件功能描述：群发文章的 URL 返回结果
    
    
    创建标识：ccccccmd - 20201013
    
----------------------------------------------------------------*/

using System.Collections.Generic;

namespace Senparc.NeuChar.Entities
{


    /// <summary>
    /// 群发文章的 URL
    /// </summary>
    public class ArticleUrlResult
    {
        public int Count { get; set; }

        public List<ArticleUrlResult_ResultList> ResultList { get; set; }

        public ArticleUrlResult()
        {
            ResultList = new List<ArticleUrlResult_ResultList>();
        }
    }

    public class ArticleUrlResult_ResultList
    {
        public ArticleUrlResult_ResultList_Item item { get; set; }

        public ArticleUrlResult_ResultList()
        {
            item = new ArticleUrlResult_ResultList_Item();
        }
    }

    public class ArticleUrlResult_ResultList_Item
    {

        public int ArticleIdx { get; set; }

        public string ArticleUrl { get; set; }
    }
}