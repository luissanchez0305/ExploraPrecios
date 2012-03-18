using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.DomainModel;
using System.Diagnostics;

namespace Explora_Precios.Core
{
    [DebuggerDisplay("{category.departament.name} -> {category.name} -> {name}")]
    public class SubCategory : Entity
    {
        [DomainSignature]
        public virtual Category category { get; set; }
        public virtual string name { get; set; }
        public virtual IList<ProductType> productTypes { get; set; }
    }
}
