using Microsoft.VisualStudio.TestTools.UnitTesting;
using Senparc.CO2NET;
using Senparc.CO2NET.ApiBind;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senparc.NeuChar.Tests
{
    [NcApiBind(Ignore = true)]
    public static class ApiBindTestClass_Ignore
    {
        public static void MethodForTest(string accessTokenOrApi, string p1, string p2)
        {
            Console.WriteLine($"accessTokenOrApi:{accessTokenOrApi} , p1:{p1} , p2 {p2}");
        }
    }

    //此处即使不忽略，但是全局已经忽略
    [NcApiBind(Ignore = false)]
    public static class ApiBindTestClass_Allow
    {
        public static void MethodForTest(string accessTokenOrApi, string p1, string p2)
        {
            Console.WriteLine($"accessTokenOrApi:{accessTokenOrApi} , p1:{p1} , p2 {p2}");
        }
    }

    [NcApiBind(Ignore = false)]
    public static class ApiBindTestClass_Default
    {
        public static void MethodForTest(string accessTokenOrApi, string p1, string p2)
        {
            Console.WriteLine($"accessTokenOrApi:{accessTokenOrApi} , p1:{p1} , p2 {p2}");
        }
    }


    [TestClass]
    public class IgnoreNcApiBindTests : BaseTest
    {
        public IgnoreNcApiBindTests() : base(ignoreNeuCharApiBind: true)
        {

        }

        [TestMethod]
        public void IgnoreApiBindTest()
        {
            var apiData = ApiBindInfoCollection.Instance;
            apiData.Keys.ToList().ForEach(key => { Console.WriteLine(key); });

            Assert.IsFalse(apiData.Keys.Any(z => z.Contains(nameof(ApiBindTestClass_Ignore))));
            Assert.IsFalse(apiData.Keys.Any(z => z.Contains(nameof(ApiBindTestClass_Default))));
            Assert.IsFalse(apiData.Keys.Any(z => z.Contains(nameof(ApiBindTestClass_Allow))));
        }
    }
}
