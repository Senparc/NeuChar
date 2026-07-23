using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;
using Senparc.NeuChar.Extensions;

namespace Senparc.NeuChar.Tests.Extensions
{
    [TestClass]
    public class NewtonSoft
    {
        [TestMethod]
        [TestCategory("CO2NET4Migration")]
        public void TryGetValueTest()
        {
            var json = @"{""UserName"":""Jeffrey Su"",""CompanyNumber"":123456,""NullProp"":null}";
            
            JsonObject item = JsonNode.Parse(json).AsObject();

            var userName = item.TryGetValue<string>("UserName");
            Assert.AreEqual("Jeffrey Su", userName);

            var initNumber = 0;
            var companyNumber = item.TryGetValue<int>("CompanyNumber", jsonNode => initNumber = jsonNode.GetValue<int>() + 654321);
            Assert.AreEqual(123456, companyNumber);
            Assert.AreEqual(777777, initNumber);

            var nullProp = item.TryGetValue<string>("NullProp");
            Assert.AreEqual(null, nullProp);

            var notExist = item.TryGetValue<int?>("notExist");
            Assert.AreEqual(null, notExist);

            var notExistZero = item.TryGetValue<int>("notExist");
            Assert.AreEqual(0, notExistZero);
        }

        [TestMethod]
        [TestCategory("CO2NET4Migration")]
        public void CoreAssemblyDoesNotReferenceNewtonsoftJsonTest()
        {
            var references = typeof(SystemTextJsonExtensions).Assembly.GetReferencedAssemblies();
            Assert.IsFalse(Array.Exists(references, reference =>
                string.Equals(reference.Name, "Newtonsoft.Json", StringComparison.OrdinalIgnoreCase)));
        }


    }
  
}
