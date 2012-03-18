using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using Explora_Precios.Core;
using FluentNHibernate.Automapping;

namespace Explora_Precios.Data.NHibernateMaps
{
    public class Catalog_AddressMap : IAutoMappingOverride<Catalog_Address>
    {
        public void Override(AutoMapping<Catalog_Address> mapping)
        {
            mapping.Id(x => x.Id).GeneratedBy.Identity();
            mapping.References(x => x.client).Not.LazyLoad();
            mapping.Map(x => x.level_Id);
            mapping.Map(x => x.catalog_Id);
            mapping.Map(x => x.url);
            mapping.Map(x => x.manualFeed);
        }
    }
}
