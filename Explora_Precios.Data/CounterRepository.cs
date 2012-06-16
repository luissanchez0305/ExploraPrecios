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
		//public IEnumerable<ProductCounter> GetChartData() {
		//    return NHibernateSession.Current.CreateCriteria(typeof(ProductCounter))
		//        .Add(Expression.Eq("Type", CounterType.Product))
		//        .List<ProductCounter>();
		//}

		private const string GeneralProductsQuery = @"SELECT c.date, c.weight, (CASE WHEN level_Id = 0 THEN catalog_Id ELSE 
										CASE WHEN level_Id = 1 THEN (SELECT c.department_Id FROM Categories c WHERE c.Id = catalog_Id) ELSE
											CASE WHEN level_Id = 2 THEN (SELECT c1.department_Id FROM SubCategories sc JOIN Categories c1 ON c1.Id = sc.category_Id WHERE sc.Id = catalog_Id) ELSE								
												CASE WHEN level_Id = 3 THEN (SELECT c2.department_Id FROM ProductTypes pt JOIN SubCategories sc1 ON sc1.Id = pt.subCategory_Id JOIN Categories c2 ON c2.Id = sc1.category_Id WHERE pt.Id = catalog_Id) 
												END
											END
										END
									END) as department_Id
						FROM Counter c JOIN
						Products p ON p.Id = c.typeId
						WHERE c.type = 0";

		public IEnumerable<ProductCounterDepartment> GetChartGeneralData()
		{
			var query = GeneralProductsQuery;

			var dataArray = NHibernateSession.Current.CreateSQLQuery(query).List();

			var dataResponse = new List<ProductCounterDepartment>();
			foreach (var obj in dataArray)
			{
				dataResponse.Add(new ProductCounterDepartment
				{
					date = (DateTime)((object[])obj)[0],
					weight = float.Parse((((object[])obj)[1]).ToString()),
					departmentId = (int)((object[])obj)[2]
				});
			}

			return dataResponse;

		}

		public IEnumerable<ProductCounterDepartment> GetChartIndividualData() {
			var query = GeneralProductsQuery + " AND c.weight = 1";

			var dataArray = NHibernateSession.Current.CreateSQLQuery(query).List();

			var dataResponse = new List<ProductCounterDepartment>();
			foreach (var obj in dataArray)
			{
				dataResponse.Add(new ProductCounterDepartment
				{
					date = (DateTime)((object[])obj)[0],
					weight = float.Parse((((object[])obj)[1]).ToString()),
					departmentId = (int)((object[])obj)[2]
				});
			}

			return dataResponse;
		}
	}

	public class ClientCounterRepository : Repository<ClientCounter>, IClientCounterRepository
	{
		private const string GeneralClientsQuery = @"SELECT c.date, c.weight, (CASE WHEN level_Id = 0 THEN catalog_Id ELSE 
										CASE WHEN level_Id = 1 THEN (SELECT c.department_Id FROM Categories c WHERE c.Id = catalog_Id) ELSE
											CASE WHEN level_Id = 2 THEN (SELECT c1.department_Id FROM SubCategories sc JOIN Categories c1 ON c1.Id = sc.category_Id WHERE sc.Id = catalog_Id) ELSE								
												CASE WHEN level_Id = 3 THEN (SELECT c2.department_Id FROM ProductTypes pt JOIN SubCategories sc1 ON sc1.Id = pt.subCategory_Id JOIN Categories c2 ON c2.Id = sc1.category_Id WHERE pt.Id = catalog_Id) 
												END
											END
										END
									END) as department_Id
						FROM Counter c JOIN
						Products p ON p.Id = c.typeId
						WHERE c.type = 1";

		public IEnumerable<ProductCounterDepartment> GetChartClientData()
		{
			var query = GeneralClientsQuery;

			var dataArray = NHibernateSession.Current.CreateSQLQuery(query).List();

			var dataResponse = new List<ProductCounterDepartment>();
			foreach (var obj in dataArray)
			{
				dataResponse.Add(new ProductCounterDepartment
				{
					date = (DateTime)((object[])obj)[0],
					weight = float.Parse((((object[])obj)[1]).ToString()),
					departmentId = (int)((object[])obj)[2]
				});
			}

			return dataResponse;
		}

		public IEnumerable<ClientCounter> GetChartData()
		{
			return NHibernateSession.Current.CreateCriteria(typeof(ClientCounter))
				.Add(Expression.Eq("Type", CounterType.Client))
				.List<ClientCounter>();
		} 
	}

}
