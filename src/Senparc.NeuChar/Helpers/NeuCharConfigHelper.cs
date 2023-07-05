using Senparc.CO2NET.Extensions;
using Senparc.CO2NET.Helpers;
using Senparc.CO2NET.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senparc.NeuChar.Helpers
{
    /// <summary>
    /// NeuCharRoot.config
    /// </summary>
    public static class NeuCharConfigHelper
    {
        /// <summary>
        /// 尝试获取当前 NeuCharRoot.config 文件的绝对路径
        /// </summary>
        /// <returns></returns>
        internal static string GetNeuCharRootConfigFilePath(string multiTenantId)
        {
            var path = ServerUtility.ContentRootMapPath("~/App_Data/NeuChar");

            FileHelper.TryCreateDirectory(path);

            if (!multiTenantId.IsNullOrEmpty())
            {
                path = Path.Combine(path, multiTenantId);
            }
            return path;
        }
    }
}
