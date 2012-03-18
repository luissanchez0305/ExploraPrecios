using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.DomainModel;

namespace Explora_Precios.Core
{
    public class Image : Entity
    {
        public virtual byte[] imageObj { get; set; }
        public virtual string url { get; set; }
    }
}
