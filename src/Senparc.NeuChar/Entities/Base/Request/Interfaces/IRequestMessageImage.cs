using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Senparc.NeuChar.Entities
{
    public interface IRequestMessageImage: IRequestMessageBase
    {
        string MediaId { get; set; }
        string PicUrl { get; set; }
    }
}
