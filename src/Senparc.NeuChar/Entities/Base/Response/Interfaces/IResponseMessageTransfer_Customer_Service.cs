using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.Entities
{
    /// <summary>
    /// 所有ResponseMessageTransfer_Customer_Service的接口
    /// </summary>
    public interface IResponseMessageTransfer_Customer_Service : IResponseMessageBase
    {

    }

    public class CustomerServiceAccount
    {
        public string KfAccount { get; set; }
    }
}
