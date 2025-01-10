using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Senparc.NeuChar.Extensions;

namespace Senparc.NeuChar.Tests.Extensions
{
    [TestClass]
    public class NewtonSoft
    {
        [TestMethod]
        public void TryGetValueTest()
        {
            var json = @"{""UserName"":""Jeffrey Su"",""CompanyNumber"":123456,""NullProp"":null}";
            
            JObject item = JObject.Parse(json);

            var userName = item.TryGetValue<string>("UserName");
            Assert.AreEqual("Jeffrey Su", userName);

            var initNumber = 0;
            var companyNumber = item.TryGetValue<int>("CompanyNumber", jToke => initNumber = jToke.ToObject<int>() + 654321);
            Assert.AreEqual(123456, companyNumber);
            Assert.AreEqual(777777, initNumber);

            var nullProp = item.TryGetValue<string>("NullProp");
            Assert.AreEqual(null, nullProp);

            var notExist = item.TryGetValue<int?>("notExist");
            Assert.AreEqual(null, notExist);

            var notExistZero = item.TryGetValue<int>("notExist");
            Assert.AreEqual(0, notExistZero);
        }


    }
  
}
