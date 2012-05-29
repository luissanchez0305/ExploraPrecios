using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Data.NHibernate;
using Explora_Precios.Core;
using Explora_Precios.Core.DataInterfaces;
using NHibernate.Criterion;

namespace Explora_Precios.Data
{
	public class ProductCounterRepository : Repository<ProductCounter>, IProductCounterRepository
	{
		public IEnumerable<ProductCounter> GetChartData() {
			return NHibernateSession.Current.CreateCriteria(typeof(ProductCounter))
				.Add(Expression.Eq("Type", CounterType.Product))
				.List<ProductCounter>();
		}
	}

	public class ClientCounterRepository : Repository<ClientCounter>, IClientCounterRepository
	{
		public IEnumerable<ClientCounter> GetChartData()
		{
			return NHibernateSession.Current.CreateCriteria(typeof(ClientCounter))
				.Add(Expression.Eq("Type", CounterType.Client))
				.List<ClientCounter>();
		} 
	}
}
