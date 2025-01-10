using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senparc.NeuChar.Utilities
{
    /// <summary>
    /// ApiUtility
    /// </summary>
    public class ApiUtility
    {
        /// <summary>
        /// 获取过期时间
        /// </summary>
        /// <param name="expireInSeconds">有效时间（秒）</param>
        /// <returns></returns>
        public static DateTimeOffset GetExpireTime(int expireInSeconds)
        {
            return SystemTime.Now.Add(GetExpiryTimeSpan(expireInSeconds));//提前x分钟重新获取
        }

        /// <summary>
        /// 获取过期 TimeSpan
        /// </summary>
        /// <param name="expireInSeconds">有效时间（秒）</param>
        /// <returns></returns>
        public static TimeSpan GetExpiryTimeSpan(int expireInSeconds)
        {
            if (expireInSeconds > 3600)
            {
                expireInSeconds -= 600;//提前10分钟过期
            }
            else if (expireInSeconds > 1800)
            {
                expireInSeconds -= 300;//提前5分钟过期
            }
            else if (expireInSeconds > 300)
            {
                expireInSeconds -= 30;//提前1分钟过期
            }
            return TimeSpan.FromSeconds(expireInSeconds);
        }
    }
}
