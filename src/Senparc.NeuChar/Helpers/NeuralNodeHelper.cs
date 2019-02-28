using Senparc.CO2NET.Extensions;
using Senparc.CO2NET.Helpers;
using Senparc.CO2NET.Trace;
using Senparc.NeuChar.Entities;
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
            return originContent.Replace("{now}", SystemTime.Now.ToString());
        }

        /// <summary>
        /// 处理图文消息
        /// </summary>
        /// <param name="originContent">id|id|id</param>
        /// <param name="data">素材资源库</param>
        /// <returns></returns>
        public static List<Article> FillNewsMessage(string originContent, MaterialData data)
        {
            if (originContent.IsNullOrWhiteSpace())
            {
                return null;
            }

            //var firstMaterialId = originContent.Split('|')?[0];


            List<Article> articleList = new List<Article>();

            //materialIds 如：9DAAC45C|6309EAD9，记录了设置当时的News的文章顺序，第一个参数为主图文的Id，
            //主图文的material内已经自带了所有关联的多图文Id，因此这里只需要取第一个
            var materialIds = originContent.Split('|');
            var material = data.FirstOrDefault(z => z.Id == materialIds[0]);
            if (material == null)
            {
                return null;
            }

            var articleData = SerializerHelper.GetObject<ArticleData>(material.Content);//获取主文章（第一篇图文）内容
            if (articleData != null && articleData.ArticleIds != null)
            {
                for (int i = 0; i < articleData.ArticleIds.Count; i++)
                {
                    ArticleData articleItem = null;
                    string materialId = null;
                    if (i == 0)
                    {
                        articleItem = articleData;//第一项就是自己
                        materialId = material.Id;
                    }
                    else
                    {
                        var materialItem = data.FirstOrDefault(z => z.Id == articleData.ArticleIds[i]);//后续选项从素材中查找
                        if (materialItem != null)
                        {
                            articleItem = SerializerHelper.GetObject<ArticleData>(materialItem.Content);
                            if (articleItem == null)
                            {
                                continue;
                            }
                            materialId = materialItem.Id;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    var article = new Article()
                    {
                        Title = articleItem?.Title,
                        PicUrl = articleItem?.ThumbCoverUrl,
                        Description = articleItem?.Digest,
                        //   Url = $"http://neuchar.senparc.com/Material/Details?uniqueId={articleItem.ArticleIds[0]}"
                    };

                    if (articleItem.ContentSourceUrl.IsNullOrWhiteSpace())
                    {
                        article.Url = $"https://www.neuchar.com/WX/Material/Details?uniqueId={materialId}";
                    }
                    else
                    {
                        article.Url = articleItem.ContentSourceUrl;
                    }

                    articleList.Add(article);

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
