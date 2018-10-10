using Senparc.CO2NET.Extensions;
using Senparc.CO2NET.Helpers;
using Senparc.CO2NET.Trace;
using Senparc.NeuChar.Entities;
using Senparc.NeuChar.MessageHandlers;
using Senparc.NeuChar.NeuralSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.Helpers
{
    /// <summary>
    /// NeuralNode 帮助类
    /// </summary>
    public static class NeuralNodeHelper
    {

        /// <summary>
        /// 处理文本信息占位符
        /// </summary>
        /// <param name="originContent"></param>
        /// <returns></returns>
        public static string FillTextMessage(string originContent)
        {
            return originContent.Replace("{now}", DateTime.Now.ToString());
        }

        /// <summary>
        /// 处理图文消息
        /// </summary>
        /// <param name="originContent">id|id|id</param>
        /// <param name="data">素材资源库</param>
        /// <returns></returns>
        public static List<Article> FillNewsMessage(string originContent, MaterialData data)
        {
            originContent = originContent ?? "";
            var firstMaterialId = originContent.Split('|')[0];
            List<Article> articleList = new List<Article>();

            Func<string, List<string>> getArticle = materialId =>
             {
                 var material = data.FirstOrDefault(z => z.Id == materialId);
                 if (material == null)
                 {
                     return null;
                 }

                 var articleData = SerializerHelper.GetObject<ArticleData>(material.Content);
                 var article = new Article()
                 {
                     Title = articleData?.Title,
                     PicUrl = articleData?.ThumbCoverUrl,
                     Description = articleData?.Digest,
                     //   Url = $"http://neuchar.senparc.com/Material/Details?uniqueId={articleData.ArticleIds[0]}"
                 };

                 if (articleData.ContentSourceUrl.IsNullOrWhiteSpace())
                 {
                     article.Url = $"http://neuchar.senparc.com/WX/Material/Details?uniqueId={material.Id}";
                 }
                 else
                 {
                     article.Url = articleData.ContentSourceUrl;
                 }

                 articleList.Add(article);
                 return articleData.ArticleIds;
             };

            var ids = getArticle(firstMaterialId);//第一篇
            if (ids != null)
            {
                foreach (var item in ids)
                {
                    getArticle(item);
                }
            }

            return articleList.Count > 0 ? articleList : null;//TODO:可以返回一条默认有好消息

            //var list = SerializerHelper.GetObject<List<Article>>(originContent);

            //foreach (var item in list)
            //{
            //    item.Title = FillTextMessage(item.Title);
            //    item.Description = FillTextMessage(item.Description);
            //}

            //return list;
        }

        /// <summary>
        /// 处理图片信息MediaId填充
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <param name="originContent"></param>
        /// <returns></returns>
        public static string GetImageMessageMediaId(IRequestMessageBase requestMessage, string originContent)
        {
            if (originContent != null && originContent.Equals("{current_img}", StringComparison.OrdinalIgnoreCase) &&
                requestMessage is IRequestMessageImage)
            {
                return (requestMessage as IRequestMessageImage).MediaId;
            }

            return originContent;
        }


        /// <summary>
        /// 获取响应素材内容
        /// </summary>
        /// <param name="materialId"></param>
        /// <param name="materialData"></param>
        /// <returns></returns>
        public static string GetMaterialContent(string materialId, MaterialData materialData)
        {
            //SenparcTrace.SendCustomLog("GetMaterialContent", $"{responseConfig.ToJson()} //// {materialData.ToJson()}");

            var materialDataItem = materialData.FirstOrDefault(z => z.Id == materialId);
            if (materialDataItem != null)
            {
                return materialDataItem.Content;
            }
            return null;
        }

        /// <summary>
        /// 获取响应素材内容
        /// </summary>
        /// <param name="responseConfig"></param>
        /// <param name="materialData"></param>
        /// <returns></returns>
        public static string GetMaterialContent(this Response responseConfig, MaterialData materialData)
        {
            //SenparcTrace.SendCustomLog("GetMaterialContent", $"{responseConfig.ToJson()} //// {materialData.ToJson()}");

            var id = responseConfig.MaterialId;
            return GetMaterialContent(id, materialData);
        }
    }
}
