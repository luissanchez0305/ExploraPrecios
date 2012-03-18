using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Explora_Precios.Core;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace Explora_Precios.Data.NHibernateMaps
{
    public class SubCategoryMap : IAutoMappingOverride<SubCategory>
    {
        public void Override(AutoMapping<SubCategory> mapping)
        {
            mapping.Id(x => x.Id).GeneratedBy.Identity();
            mapping.Map(x => x.name);
            mapping.HasMany(x => x.productTypes)
                .Cascade.AllDeleteOrphan()
                .OrderBy("name");
        }
    }
}
