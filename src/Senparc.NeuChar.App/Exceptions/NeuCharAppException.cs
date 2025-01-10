using Senparc.CO2NET.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senparc.NeuChar.App.Exceptions
{
    /// <summary>
    /// NeuCharApp 异常
    /// </summary>
    public class NeuCharAppException : BaseException
    {
        public NeuCharAppException(string message)
          : this(message, null)
        {
        }

        public NeuCharAppException(string message, Exception inner)
          : base(message, inner)
        {
        }

    }
}