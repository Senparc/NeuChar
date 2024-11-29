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

        [TestMethod]
        public void ChunkStringByUnicode()
        {
            var limit = 10;
            var text = "Senparc.NeuChar🤝 跨平台信息交互🔄标准🌍。使用 NeuChar 标准💬可以跨平台🌐兼容不同平台的交互🔀信息设置，一次设置🚀，多平台共享📱🖥。";

            var results = MessageHandlerHelper.ChunkStringByUnicode(text, limit);

            foreach (var result in results)
            {
                var bytes = Encoding.UTF8.GetBytes(result);
                Assert.IsTrue(bytes.Length <= limit);
            }
        }

        [TestMethod]
        public async Task HandleLimitedTextAsync()
        {
            var limit = 10;
            var text = "Senparc.NeuChar🤝 跨平台信息交互🔄标准🌍。使用 NeuChar 标准💬可以跨平台🌐兼容不同平台的交互🔀信息设置，一次设置🚀，多平台共享📱🖥。";

            var results = await MessageHandlerHelper.TryHandleLimitedText(text, limit, chunk =>
            {
                return Task.FromResult(chunk);
            });

            Assert.AreEqual(text, string.Join(string.Empty, results));
        }

    }
}