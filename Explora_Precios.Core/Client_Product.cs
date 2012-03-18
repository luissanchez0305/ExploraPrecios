using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.DomainModel;
using Explora_Precios.Core.Helper;
using System.Diagnostics;

namespace Explora_Precios.Core
{
    [Auditable]
    [DebuggerDisplay("{client.name} - {productReference} - {price} - {specialPrice}")]
    public class Client_Product : Entity
    {
        [DomainSignature]
        public virtual Client client { get; set; }
        public virtual Product product { get; set; }
        public virtual string productReference { get; set; }    
        public virtual int counter { get; set; }
        public virtual float price { get; set; }
        public virtual float specialPrice { get; set; }
        public virtual string url { get; set; }
        public virtual bool isActive { get; set; }
        public virtual string name { get; set; }
        public virtual DateTime dateCreated { get; set; }
        public virtual DateTime dateModified { get; set; }
        public virtual DateTime dateReported { get; set; }
        public virtual string page { get; set; }
        public virtual bool isHighlighted { get; set; }
    }
}
