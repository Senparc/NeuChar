using Microsoft.VisualStudio.TestTools.UnitTesting;
using Senparc.CO2NET.RegisterServices;
using System;
using System.Collections.Generic;
using System.Text;

namespace Senparc.NeuChar.Tests
{
    public class ApiTest: BaseTest
    {

        public void MethodForTest(string accessTokenOrApi,string p1,string p2)
        {
            Console.WriteLine($"accessTokenOrApi:{accessTokenOrApi} , p1:{p1} , p2 {p2}");
        }
    }

    [TestClass]
    public class RegisterTests
    {
        [TestMethod]
        public void RegisterApiBindTest()
        {
            Register.RegisterApiBind();

            Assert.IsTrue(Register.ApiBindInfoCollection.Count > 0);
        }

    }
}
