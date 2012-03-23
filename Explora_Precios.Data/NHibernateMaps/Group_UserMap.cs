using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using Explora_Precios.Core;
using FluentNHibernate.Automapping;

namespace Explora_Precios.Data.NHibernateMaps
{
    public class Group_UserMap : IAutoMappingOverride<Group_User>
    {
        public void Override(AutoMapping<Group_User> mapping)
        {
            mapping.Id(x => x.Id).GeneratedBy.Identity();
            mapping.References(x => x.user);
            mapping.References(x => x.product);
            mapping.Map(x => x.created);
        }
    }
}
