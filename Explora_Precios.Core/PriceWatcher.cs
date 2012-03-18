using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.DomainModel;

namespace Explora_Precios.Core
{
    public class PriceWatcher : Entity
    {
        [DomainSignature]
        public virtual Product product { get; set; }
        public virtual User user { get; set; }
        public virtual float previousPrice { get; set; }
        public virtual float currentPrice { get; set; }
    }
}
