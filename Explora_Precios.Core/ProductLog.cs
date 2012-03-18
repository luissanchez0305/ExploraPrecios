using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Event;
using System.Security.Principal;
using NHibernate.Persister.Entity;
using SharpArch.Core.DomainModel;

namespace Explora_Precios.Core
{
    public class ProductLog : Entity
    {
        public enum ProductLogEntryOperations
        {
            Insert,
            Update,
            Delete
        }

        [DomainSignature]
        public virtual int client_Id { get; set; }
        public virtual int product_Id { get; set; }
        public virtual DateTime changeDate { get; set; }
        public virtual ProductLogEntryOperations operation { get; set; }
        public virtual float fromPrice { get; set; }
        public virtual float toPrice { get; set; }
        public virtual float fromSpecialPrice { get; set; }
        public virtual float toSpecialPrice { get; set; }
        public virtual int user_Id { get; set; }
    }
}
