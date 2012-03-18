using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using Explora_Precios.Core;
using FluentNHibernate.Automapping;

namespace Explora_Precios.Data.NHibernateMaps
{
    public class Client_ProductMap : IAutoMappingOverride<Client_Product>
    {
        public void Override(AutoMapping<Client_Product> mapping)
        {
            mapping.Id(x => x.Id).GeneratedBy.Identity();
            mapping.References(x => x.client);
            mapping.References(x => x.product);
            mapping.Map(x => x.productReference);
            mapping.Map(x => x.name, "productName");
            mapping.Map(x => x.counter);
            mapping.Map(x => x.price);
            mapping.Map(x => x.specialPrice);
            mapping.Map(x => x.isActive);
            mapping.Map(x => x.dateCreated);
            mapping.Map(x => x.dateModified);
            mapping.Map(x => x.page);
            mapping.Map(x => x.isHighlighted);
        }
    }
}
