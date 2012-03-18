using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using Explora_Precios.Core;
using FluentNHibernate.Automapping;

namespace Explora_Precios.Data.NHibernateMaps
{
    public class DepartamentMap : IAutoMappingOverride<Department>
    {
        public void Override(AutoMapping<Department> mapping)
        {
            mapping.Id(x => x.Id).GeneratedBy.Identity();
            mapping.Map(x => x.name);
            mapping.HasMany(x => x.categories)
                .Cascade.AllDeleteOrphan()
                .OrderBy("name");
        }
    }
}
