using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.DomainModel;

namespace Explora_Precios.Core
{
    public class Product_Quality : Entity
    {
        [DomainSignature]
        public virtual Product product { get; set; }
        public virtual Quality quality { get; set; }
        public virtual string value { get; set; }
    }
}
