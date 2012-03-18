using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.DomainModel;
using System.Diagnostics;

namespace Explora_Precios.Core
{
    [DebuggerDisplay("{departament.name} -> {name}")]
    public class Category : Entity
    {
        [DomainSignature]
        public virtual Department department { get; set; }
        public virtual string name { get; set; }
        public virtual IList<SubCategory> subCategories { get; set; }
    }
}
