using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Explora_Precios.Core;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Automapping;

namespace Explora_Precios.Data.NHibernateMaps
{
	public class ProductCounterMap : IAutoMappingOverride<ProductCounter>
	{
		public void Override(AutoMapping<ProductCounter> mapping)
		{
			mapping.Table("Counter");
			mapping.Id(x => x.Id).GeneratedBy.Identity();
			mapping.Map(x => x.Type).CustomType(typeof(Explora_Precios.Core.CounterType));
			mapping.References(x => x.product, "typeId");
			mapping.Map(x => x.weight);
			mapping.Map(x => x.date);
		}
	}

	public class ClientCounterMap : IAutoMappingOverride<ClientCounter>
	{
		public void Override(AutoMapping<ClientCounter> mapping)
		{
			mapping.Table("Counter");
			mapping.Id(x => x.Id).GeneratedBy.Identity();
			mapping.Map(x => x.Type).CustomType(typeof(Explora_Precios.Core.CounterType));
			mapping.References(x => x.client, "typeId");
			mapping.Map(x => x.weight);
			mapping.Map(x => x.date);
		}
	}
}
