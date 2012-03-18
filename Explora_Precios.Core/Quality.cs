using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.DomainModel;

namespace Explora_Precios.Core
{
    public class Quality : Entity
    {
        [DomainSignature]
        public virtual string name { get; set; }
        public virtual IList<Product_Quality> products { get; set; }
    }
}
