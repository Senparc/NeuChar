using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senparc.NeuChar.App.Entities
{
    /// <summary>
    /// 给 NeuCharAppController 使用的 PostModel
    /// </summary>
    public class PostModel : EncryptPostModel
    {
        public override string DomainId { get => AppId; set => AppId = value; }

        /// <summary>
        /// 当前请求对应的 NeuChar App 的 AppId
        /// </summary>
        public string AppId { get; set; }
    }
}
