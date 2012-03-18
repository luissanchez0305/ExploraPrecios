using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.DomainModel;

namespace Explora_Precios.Core
{
    public class Catalog_Address : Entity
    {
        [DomainSignature]
        public virtual Client client { get; set; }
        public virtual int level_Id { get; set; }
        public virtual int catalog_Id { get; set; }
        public virtual string url { get; set; }
        public virtual bool manualFeed { get; set; }
    }
}
