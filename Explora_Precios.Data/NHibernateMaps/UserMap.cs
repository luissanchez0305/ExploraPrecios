using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using Explora_Precios.Core;
using FluentNHibernate.Automapping;

namespace Explora_Precios.Data.NHibernateMaps
{
    public class UserMap : IAutoMappingOverride<User>
    {
        public void Override(AutoMapping<User> mapping)
        {
            mapping.Id(x => x.Id, "userId").GeneratedBy.Identity();
            mapping.Map(x => x.email, "email");
            mapping.Map(x => x.username, "username"); 
            mapping.References(x => x.city).Not.LazyLoad();
            mapping.Map(x => x.gender, "gender");
            mapping.Map(x => x.name, "name");
            mapping.Map(x => x.lastName, "lastName");
            mapping.Map(x => x.validationCode, "validationCode");
            mapping.Map(x => x.birthdate, "birthdate");
            mapping.Map(x => x.isApproved, "isApproved");
            mapping.Map(x => x.facebookToken, "facebookToken");
            mapping.HasManyToMany(x => x.groups)
                .Table("Group_Users")
                .ParentKeyColumn("user_Id")
                .ChildKeyColumn("product_Id")
                .Cascade.AllDeleteOrphan();
        }
    }
}
