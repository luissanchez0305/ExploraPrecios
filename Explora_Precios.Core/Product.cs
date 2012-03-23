using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.DomainModel;
using System.Diagnostics;

namespace Explora_Precios.Core
{
    [DebuggerDisplay("{name} - {productReference} - {brand.name}")]
    public class Product : Entity
    {
        public Product() 
        { 
            qualities = new List<Product_Quality>();
            clients = new List<Client_Product>();
            ratings = new List<User_Product>();
        }
        [DomainSignature]
        public virtual string productReference { get; set; }
        public virtual string name { get; set; }
        public virtual int level_Id { get; set; }
        public virtual int catalog_Id { get; set; }
        public virtual Brand brand { get; set; }
        public virtual Image image { get; set; }
        public virtual string description { get; set; }
        public virtual IList<Product_Quality> qualities { get; set; }
        public virtual IList<Client_Product> clients { get; set; }
        public virtual IList<User_Product> ratings { get; set; }
        public virtual IList<Group_User> groups { get; set; }
    }
}
