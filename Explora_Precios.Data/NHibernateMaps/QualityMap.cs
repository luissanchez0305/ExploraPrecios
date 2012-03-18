using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using Explora_Precios.Core;
using FluentNHibernate.Automapping;

namespace Explora_Precios.Data.NHibernateMaps
{
    public class QualityMap : IAutoMappingOverride<Quality>
    {
        public void Override(AutoMapping<Quality> mapping)
        {
            mapping.Id(x => x.Id).GeneratedBy.Identity();
            mapping.Map(x => x.name);
            mapping.HasManyToMany(x => x.products)
                .Table("Product_Qualities")
                .ParentKeyColumn("quality_Id")
                .ChildKeyColumn("product_Id");
                
        }
    }
}
