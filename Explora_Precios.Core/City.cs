using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.DomainModel;

namespace Explora_Precios.Core
{
    public class City : Entity
    {
        [DomainSignature]
        public virtual Country country { get; set; }
        public virtual string name { get; set; }
    }

    public class Country : Entity
    {
        [DomainSignature]
        public virtual string name { get; set; }
    }
}
