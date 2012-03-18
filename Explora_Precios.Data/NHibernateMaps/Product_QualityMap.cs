using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using Explora_Precios.Core;
using FluentNHibernate.Automapping;

namespace Explora_Precios.Data.NHibernateMaps
{
    public class Product_QualityMap : IAutoMappingOverride<Product_Quality>
    {
        public void Override(AutoMapping<Product_Quality> mapping)
        {
            mapping.Id(x => x.Id, "Id").GeneratedBy.Identity();
            mapping.References(x => x.quality, "quality_Id");
            mapping.References(x => x.product, "product_Id");
            mapping.Map(x => x.value);
        }
    }
}
