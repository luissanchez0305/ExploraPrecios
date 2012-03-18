using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentNHibernate.Automapping.Alterations;
using Explora_Precios.Core;
using FluentNHibernate.Automapping;

namespace Explora_Precios.Data
{
    public class ContactCommentsMaps : IAutoMappingOverride<ContactComments>
    {
        public void Override(AutoMapping<ContactComments> mapping) {
            mapping.Id(x => x.Id).GeneratedBy.Identity();
            mapping.Map(x => x.name);
            mapping.Map(x => x.email);
            mapping.Map(x => x.message);
            mapping.Map(x => x.dateCreate);
            mapping.Map(x => x.answered);
        }
    }
}
