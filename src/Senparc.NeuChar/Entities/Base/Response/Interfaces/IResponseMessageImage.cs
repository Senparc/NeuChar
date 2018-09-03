using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.Entities
{
    public interface IResponseMessageImage: IResponseMessageBase
    {
        Image Image { get; set; }
    }
}
