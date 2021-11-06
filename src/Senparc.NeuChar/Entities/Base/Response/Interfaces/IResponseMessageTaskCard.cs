using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senparc.NeuChar.Entities
{
    
    public interface IResponseMessageTaskCard : IResponseMessageBase
    {
        TaskCard TaskCard { get; set; }
    }
}
