using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.ApiHandlers
{
    public class ApiResult
    {
        public int ResultCode { get; set; }
        public string ResultMessage { get; set; }
        public object ResultObject { get; set; }

        public ApiResult()
        { }


        public ApiResult(int resultCode, string resultMessage, object resultObject)
        {
            ResultCode = resultCode;
            ResultMessage = resultMessage;
            ResultObject = resultObject;

        }
    }
}
