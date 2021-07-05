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
        protected  IRegisterService registerService;
        protected  IServiceCollection serviceCollection;
        public BaseTest()
        {
            RegisterServiceCollection();

            RegisterServiceStart();
        }

        /// <summary>
        /// 注册 IServiceCollection 和 MemoryCache
        /// </summary>
        public  void RegisterServiceCollection()
        {
            serviceCollection = new ServiceCollection();
            var configBuilder = new ConfigurationBuilder();
            var config = configBuilder.Build();
            serviceCollection.AddSenparcGlobalServices(config);
            serviceCollection.AddMemoryCache();//使用内存缓存
        }

        /// <summary>
        /// 注册 RegisterService.Start()
        /// </summary>
        public  void RegisterServiceStart(bool autoScanExtensionCacheStrategies = false)
        {
            //注册
            var mockEnv = new Mock<Microsoft.Extensions.Hosting.IHostEnvironment/*IHostingEnvironment*/>();
            mockEnv.Setup(z => z.ContentRootPath).Returns(() => UnitTestHelper.RootPath);
            registerService = Senparc.CO2NET.AspNet.RegisterServices.RegisterService.Start(mockEnv.Object, new SenparcSetting() { IsDebug = true })
                .UseSenparcGlobal(autoScanExtensionCacheStrategies);
        }
    }
}
