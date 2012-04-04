using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using Explora_Precios.Core;
using FluentNHibernate.Automapping;
using FluentNHibernate.Mapping;

namespace Explora_Precios.Data.NHibernateMaps
{
    public class ImageMap : IAutoMappingOverride<Image>
    {
        public void Override(AutoMapping<Image> mapping)
        {
            mapping.Id(x => x.Id).GeneratedBy.Identity();
			mapping.Map(x => x.url).LazyLoad();
			mapping.Map(x => x.imageObj).LazyLoad();
        }
    }
}
