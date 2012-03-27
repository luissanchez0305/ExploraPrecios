using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Explora_Precios.Core;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace Explora_Precios.Data.NHibernateMaps
{
    public class ProductMap : IAutoMappingOverride<Product>
    {
        public void Override(AutoMapping<Product> mapping) 
        {
            mapping.Id(x => x.Id).GeneratedBy.Identity();
            mapping.Map(x => x.productReference);
            mapping.Map(x => x.name);
            mapping.Map(x => x.catalog_Id);
            mapping.Map(x => x.level_Id);
            mapping.References(x => x.brand).Cascade.SaveUpdate();
			mapping.References(x => x.image).Cascade.SaveUpdate();
            mapping.Map(x => x.description);
            mapping.HasMany(x => x.qualities)
                .Cascade.SaveUpdate()
                .Cascade.AllDeleteOrphan()
                .Inverse()
                .AsBag();
            mapping.HasMany(x => x.clients)
                .Cascade.SaveUpdate()
                .Cascade.AllDeleteOrphan()
                .Inverse()
                .AsBag();
            mapping.HasMany(x => x.ratings)
                .Cascade.SaveUpdate()
                .Cascade.AllDeleteOrphan()
                .Inverse()
                .AsBag();
            mapping.HasMany(x => x.groups)
                .Cascade.SaveUpdate()
                .Inverse()
                .AsBag();
        }
    }
}
