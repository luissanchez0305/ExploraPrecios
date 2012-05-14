using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Explora_Precios.Core;
using SharpArch.Data.NHibernate;
using NHibernate.Criterion;
using Explora_Precios.Core.DataInterfaces;

namespace Explora_Precios.Data
{
	public class Client_ProductRepository : Repository<Client_Product>, IClient_ProductRepository
	{
		public IList<Client_Product> GetAllActive()
		{
			return NHibernateSession.Current.CreateCriteria(typeof(Client_Product))
				.Add(Expression.Eq("isActive", true))
				.List<Client_Product>();
		}

		public IList<Client_Product> GetLastAdded()
		{
			return NHibernateSession.Current.CreateCriteria(typeof(Client_Product))
				.Add(Expression.And(Expression.Ge("dateCreated", DateTime.Now.AddDays(-14)), Expression.Eq("isActive", true)))
				.List<Client_Product>();
		}
		public IList<Client_Product> GetLastUpdated()
		{
			return NHibernateSession.Current.CreateCriteria(typeof(Client_Product))
				.Add(Expression.Conjunction()
						.Add(Expression.Ge("dateModified", DateTime.Now.AddDays(-14)))
						.Add(Expression.Eq("isActive", true))
						.Add(Expression.Le("price", (float)200)))
				.SetMaxResults(20)
				.List<Client_Product>();
		}
		public IList<Client_Product> GetByClient(Client client)
		{
			return NHibernateSession.Current.CreateCriteria(typeof(Client_Product))
				.Add(Expression.Eq("client", client))
				.List<Client_Product>();
		}

		public void Update(Client_Product client_product)
		{
			NHibernate.ISession session = SharpArch.Data.NHibernate.NHibernateSession.Current;

			using (NHibernate.ITransaction transaction = session.BeginTransaction())
			{
				session.Update(client_product);
				transaction.Commit();
			}
		}

		public IList<Client_Product> GetProductsOnSale()
		{
			return NHibernateSession.Current.CreateCriteria(typeof(Client_Product))
				.Add(Expression.Conjunction()
					.Add(Expression.Gt("specialPrice", (float)0))
					.Add(Expression.Eq("isActive", true))
					.Add(Expression.GeProperty("dateModified", "dateReported")))
				.List<Client_Product>();
		}

	}
}
