using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Explora_Precios.Core;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace Explora_Precios.Data.NHibernateMaps
{
    public class CityMap : IAutoMappingOverride<City>
    {
        public void Override(AutoMapping<City> mapping)
        {
            mapping.Id(x => x.Id).GeneratedBy.Identity();
            mapping.References(x => x.country).Not.LazyLoad();
            mapping.Map(x => x.name);
        }
    }

    public class PaisMap : IAutoMappingOverride<Country>
    {
        public void Override(AutoMapping<Country> mapping)
        {
            mapping.Id(x => x.Id).GeneratedBy.Identity();
            mapping.Map(x => x.name);
        }
    }
}
