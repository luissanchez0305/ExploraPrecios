using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using Explora_Precios.Core;
using FluentNHibernate.Automapping;

namespace Explora_Precios.Data.NHibernateMaps
{
    public class ClientMap : IAutoMappingOverride<Client>
    {
        public void Override(AutoMapping<Client> mapping)
        {
            mapping.Id(x => x.Id).GeneratedBy.Identity();
            mapping.Map(x => x.name);
            mapping.Map(x => x.url);
            mapping.Map(x => x.isActive);
            mapping.Map(x => x.catalogAddress);
            mapping.HasMany(x => x.products)
                .Table("Client_Products")
                .KeyColumn("client_Id")
                .Cascade.All();
            mapping.HasMany(x => x.catalog)
                .Table("Catalog_Addresses")
                .KeyColumn("client_Id")
                .Cascade.All();
        }
    }
}
