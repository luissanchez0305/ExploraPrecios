using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;
using Explora_Precios.Core;

namespace Explora_Precios.Data.NHibernateMaps
{
    public class ProductLogMap : IAutoMappingOverride<ProductLog>
    {
        public void Override(AutoMapping<ProductLog> mapping)
        {
            mapping.Id(x => x.Id).GeneratedBy.Identity();
            mapping.Map(x => x.operation).CustomType(typeof(ProductLog.ProductLogEntryOperations));
            mapping.Map(x => x.user_Id);
            mapping.Map(x => x.changeDate);
            mapping.Map(x => x.fromPrice);
            mapping.Map(x => x.toPrice);
            mapping.Map(x => x.fromSpecialPrice);
            mapping.Map(x => x.toSpecialPrice);
        }
    }
}
