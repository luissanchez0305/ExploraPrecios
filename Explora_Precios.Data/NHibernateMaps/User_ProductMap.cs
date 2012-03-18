using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using Explora_Precios.Core;
using FluentNHibernate.Automapping;

namespace Explora_Precios.Data.NHibernateMaps
{
    public class User_ProductMap : IAutoMappingOverride<User_Product>
    {
        public void Override(AutoMapping<User_Product> mapping)
        {
            mapping.Id(x => x.Id, "userProductId").GeneratedBy.Identity();
            mapping.References(x => x.user).Not.LazyLoad(); 
            mapping.References(x => x.product).Not.LazyLoad(); 
            mapping.Map(x => x.Type, "relation_Type").CustomType(typeof(Explora_Precios.Core.User_Product.RelationType));
            mapping.Map(x => x.value, "value");
            mapping.Map(x => x.Notified, "notified");
            mapping.Map(x => x.NotifiedValue, "notified_Value");
        }
    }
}
