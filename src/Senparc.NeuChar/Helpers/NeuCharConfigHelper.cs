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
        /// 尝试获取 NeuCharRoot.config 根路径
        /// </summary>
        /// <returns></returns>
        internal static string GetNeuCharRootConfigBasePath()
        {
            return ServerUtility.ContentRootMapPath("~/App_Data/NeuChar");

        }

        /// <summary>
        /// 尝试获取当前 NeuCharRoot.config 文件的绝对路径
        /// </summary>
        /// <returns></returns>
        internal static string GetNeuCharRootConfigFilePath(string multiTenantId, bool autoCreateDirectory = true)
        {
            var path = GetNeuCharRootConfigBasePath();

            FileHelper.TryCreateDirectory(path);

            multiTenantId = TryGetDefaultMultiTenantId(multiTenantId);
            path = Path.Combine(path, multiTenantId);

            if (autoCreateDirectory)
            {
                FileHelper.TryCreateDirectory(path);
            }

            return path;
        }

        /// <summary>
        /// 尝试获取合法的 multiTenantId，如果为空，则返回默认值
        /// </summary>
        /// <param name="multiTenantId"></param>
        /// <returns></returns>
        internal static string TryGetDefaultMultiTenantId(string multiTenantId)
        {
            if (multiTenantId.IsNullOrEmpty())
            {
                //没有提供 MultiTenantId，在使用默认目录
                multiTenantId = NeuralSystem.DEFAULT_MULIT_TENANT_ID;
                return multiTenantId;
            }

            return multiTenantId;
        }
    }
}
