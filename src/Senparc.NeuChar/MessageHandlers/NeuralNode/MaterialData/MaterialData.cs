using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.NeuralSystems
{
    /// <summary>
    /// 素材数据库
    /// </summary>
    public class MaterialData : List<MaterialDataItem>
    {

    }


    /// <summary>
    /// 素材数据库
    /// </summary>
    public class MaterialDataItem : IConfigItem
    {

        #region IConfigItem
        /// <summary>
        /// 备注名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 全局唯一编号（类似Guid）
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// 版本号，如："2018.9.27.1"
        /// </summary>
        public string Version { get; set; }
        #endregion


        /// <summary>
        /// 素材类型
        /// </summary>
        public ResponseMsgType Type { get; set; }
        /// <summary>
        /// 素材内容
        /// <para>如果是多图文（News）类型，则对应ArticleData类型的JSON数据</para>
        /// </summary>
        public string Content { get; set; }
    }
}
