using FluentNHibernate.Conventions;
using FluentNHibernate.Mapping;
using FluentNHibernate.Conventions.Helpers;

namespace Explora_Precios.Data.NHibernateMaps.Conventions
{
    public class HasManyConvention : IHasManyConvention
    {
        public void Apply(FluentNHibernate.Conventions.Instances.IOneToManyCollectionInstance instance)
        {
            instance.Key.Column(instance.EntityType.Name + "ID");
        }
    }
}
