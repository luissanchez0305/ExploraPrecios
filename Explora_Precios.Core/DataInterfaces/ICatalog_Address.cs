using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.PersistenceSupport;

namespace Explora_Precios.Core.DataInterfaces
{
    public interface ICatalog_AddressRepository : IRepository<Catalog_Address>
    {
        IList<Catalog_Address> GetByClient(Client client);
        IList<Catalog_Address> GetByClientCatalogDetails(Client client, int level_Id, int catalog_Id);
        void DeleteCatalogItem(int item_Id);
    }
}
