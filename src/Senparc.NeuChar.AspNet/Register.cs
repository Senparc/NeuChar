//#if !NET45
//using Microsoft.Extensions.DependencyInjection;
//using Senparc.CO2NET.WebApi;
////using Senparc.CO2NET.WebApi.WebApiEngines;
//#endif
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Reflection.Emit;
//using System.Text;
//using System.Threading.Tasks;

//namespace Senparc.NeuChar.AspNet
//{
//    public static class Register
//    {
////#if NET45
////        /// <summary>
////        /// 注册 NeuChar
////        /// </summary>
////        /// <returns></returns>
////        public static void AddDynamicApi()
////        {
////        }
////#else
////        /// <summary>
////        /// 添加 DynamicApi 服务
////        /// </summary>
////        /// <param name="services"></param>
////        /// <param name="builder"></param>
////        /// <param name="appDataPath"></param>
////        /// <param name="defaultRequestMethod"></param>
////        /// <param name="taskCount"></param>
////        /// <param name="showDetailApiLog"></param>
////        /// <param name="copyCustomAttributes"></param>
////        /// <param name="additionalAttributeFunc"></param>
////        /// <returns></returns>
////        public static IServiceCollection AddDynamicApi(this IServiceCollection services, IMvcCoreBuilder builder,
////            string appDataPath, ApiRequestMethod defaultRequestMethod = ApiRequestMethod.Post, int taskCount = 4, bool showDetailApiLog = false, bool copyCustomAttributes = true, Func<MethodInfo, IEnumerable<CustomAttributeBuilder>> additionalAttributeFunc = null)
////        {
////            services.AddAndInitDynamicApi(builder, appDataPath, defaultRequestMethod, taskCount, showDetailApiLog, copyCustomAttributes, additionalAttributeFunc);
////            return services;
////        }
////#endif
//    }
//}
