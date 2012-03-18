using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.DomainModel;

namespace Explora_Precios.Core
{
    public class ContactComments : Entity
    {
        [DomainSignature]
        public virtual string name { get; set; }
        public virtual string email { get; set; }
        public virtual string message { get; set; }
        public virtual DateTime dateCreate { get; set; }
        public virtual bool answered { get; set; }
    }
}
