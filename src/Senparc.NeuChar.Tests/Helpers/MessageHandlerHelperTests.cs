using Microsoft.VisualStudio.TestTools.UnitTesting;
using Senparc.NeuChar.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senparc.NeuChar.Helpers.Tests
{
    [TestClass()]
    public class MessageHandlerHelperTests
    {
        [TestMethod()]
        public void SubstringByByteTest()
        {
            var origStr = "你好盛派";
            var result = MessageHandlerHelper.SubstringByByte(origStr, 4);
            Console.WriteLine(result);
            Assert.AreEqual("你", result);

            origStr = "Abc";
            result = MessageHandlerHelper.SubstringByByte(origStr, 2);
            Console.WriteLine(result);
            Assert.AreEqual("Ab", result);

            origStr = "A你b好c盛d派";
            result = MessageHandlerHelper.SubstringByByte(origStr, 6);
            Console.WriteLine(result);
            Assert.AreEqual("A你b", result);//第6个字节正好在“好”的一半，所以只取到 b

            origStr = "。。。。。。";
            result = MessageHandlerHelper.SubstringByByte(origStr, 3);
            Console.WriteLine(result);
            Assert.AreEqual("。", result);


        }
    }
}