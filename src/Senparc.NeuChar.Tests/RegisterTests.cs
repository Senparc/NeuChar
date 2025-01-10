using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Senparc.NeuChar.Tests
{

    public static class TestApi
    {
        [NcApiBind(PlatformType.WeChat_OfficialAccount, "CustomApi.SendText", true)]
        public static void MethodForTest(string accessTokenOrApi, string p1, string p2)
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
            base.serviceCollection.AddNeuChar();

            Assert.IsTrue(Register.NeuralNodeRegisterCollection.Count > 0);
            Assert.IsTrue(Register.IgnoreNeuCharApiBind);

            base.serviceCollection.AddNeuChar(false);
            Assert.IsFalse(Register.IgnoreNeuCharApiBind);
        }
    }
}
