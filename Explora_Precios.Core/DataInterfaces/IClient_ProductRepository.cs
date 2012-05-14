using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.PersistenceSupport;

namespace Explora_Precios.Core.DataInterfaces
{
	public interface IClient_ProductRepository : IRepository<Client_Product>
	{
		IList<Client_Product> GetLastAdded();
		IList<Client_Product> GetByClient(Client client);
		IList<Client_Product> GetProductsOnSale();
		void Update(Client_Product client_product);
		IList<Client_Product> GetAllActive();
		IList<Client_Product> GetLastUpdated();
	}
}
