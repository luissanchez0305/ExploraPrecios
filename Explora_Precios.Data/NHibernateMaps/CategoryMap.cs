using Explora_Precios.Core;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace Explora_Precios.Data.NHibernateMaps
{
    public class CategoryMap : IAutoMappingOverride<Category>
    {
        public void Override(AutoMapping<Category> mapping)
        {
            mapping.Id(x => x.Id).GeneratedBy.Identity();            
            mapping.Map(x => x.name);
            mapping.HasMany(x => x.subCategories)
                .Cascade.AllDeleteOrphan()
                .OrderBy("name");
        }
    }
}
