using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using Explora_Precios.Core;
using FluentNHibernate.Automapping;

namespace Explora_Precios.Data.NHibernateMaps
{
    public class PriceWatcherMap : IAutoMappingOverride<PriceWatcher>
    {
        public void Override(AutoMapping<PriceWatcher> mapping)
        {
            mapping.Id(x => x.Id).GeneratedBy.Identity();
            mapping.References(x => x.product).Not.LazyLoad();
            mapping.References(x => x.user).Not.LazyLoad();
            mapping.Map(x => x.previousPrice);
            mapping.Map(x => x.currentPrice);
        }
    }
}
