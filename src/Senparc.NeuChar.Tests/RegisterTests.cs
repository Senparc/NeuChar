using Microsoft.VisualStudio.TestTools.UnitTesting;
using Senparc.CO2NET.RegisterServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Senparc.NeuChar.Tests
{

    public static class TestApi
    {
        [ApiBind(PlatformType.WeChat_OfficialAccount, "CustomApi.SendText", true)]
        public static void MethodForTest(string accessTokenOrApi,string p1,string p2)
        {
            Console.WriteLine($"accessTokenOrApi:{accessTokenOrApi} , p1:{p1} , p2 {p2}");
        }
    }

    [TestClass]
    public class RegisterTests : BaseTest
    {
        [TestMethod]
        public void RegisterApiBindTest()
        {
            Register.RegisterApiBind();

            Assert.IsTrue(Register.NeuralNodeRegisterCollection.Count > 0);
        }

    }
}
