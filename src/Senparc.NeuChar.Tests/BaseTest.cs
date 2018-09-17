using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Senparc.CO2NET;
using Senparc.CO2NET.RegisterServices;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;

namespace Senparc.NeuChar.Tests
{
    public class BaseTest
    {
        protected static IRegisterService registerService;

        public BaseTest()
        {
            RegisterServiceCollection();

            RegisterServiceStart();
        }

        /// <summary>
        /// 注册 IServiceCollection 和 MemoryCache
        /// </summary>
        public static void RegisterServiceCollection()
        {
            var serviceCollection = new ServiceCollection();
            var configBuilder = new ConfigurationBuilder();
            var config = configBuilder.Build();
            serviceCollection.AddSenparcGlobalServices(config);
            serviceCollection.AddMemoryCache();//使用内存缓存
        }

        /// <summary>
        /// 注册 RegisterService.Start()
        /// </summary>
        public static void RegisterServiceStart(bool autoScanExtensionCacheStrategies = false)
        {
            //注册
            var mockEnv = new Mock<IHostingEnvironment>();
            mockEnv.Setup(z => z.ContentRootPath).Returns(() => UnitTestHelper.RootPath);
            registerService = RegisterService.Start(mockEnv.Object, new SenparcSetting() { IsDebug = true })
                .UseSenparcGlobal(autoScanExtensionCacheStrategies);
        }
    }
}
