using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.DomainModel;
using System.Diagnostics;

namespace Explora_Precios.Core
{
    [DebuggerDisplay("{subCategory.category.department.name} -> {subCategory.category.name} -> {subCategory.name} -> {name}")]
    public class ProductType : Entity
    {
        [DomainSignature]
        public virtual SubCategory subCategory { get; set; }
        public virtual string name { get; set; }
    }
}
