using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using Explora_Precios.Core;
using FluentNHibernate.Automapping;

namespace Explora_Precios.Data.NHibernateMaps
{
    public class MaritalStatusMap : IAutoMappingOverride<MaritalStatus>
    {
        public void Override(AutoMapping<MaritalStatus> mapping)
        {
            mapping.Id(x => x.Id).GeneratedBy.Identity();
            mapping.Map(x => x.name);
        }
    }
}
