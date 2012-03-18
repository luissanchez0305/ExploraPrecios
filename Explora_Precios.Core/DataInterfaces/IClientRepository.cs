using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Core.PersistenceSupport;
using Explora_Precios.Core;

namespace Explora_Precios.Core.DataInterfaces
{
    public interface IClientRepository : IRepository<Client>
    {
        Client GetByAddress(string address);
        Client GetByUrl(string url);
        Client GetByUrl(string url, out string domain);
        //Client GetClientWithCatalogId(Catalog_Address catalog);
    }
}
