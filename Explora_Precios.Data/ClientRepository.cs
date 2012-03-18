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
    public class ClientRepository : Repository<Client>, IClientRepository
    {
        public Client GetByAddress(string address)
        {
            return NHibernateSession.Current.CreateCriteria(typeof(Client))
                .Add(Expression.Eq("url", address))
                .UniqueResult<Client>();
        }
        public Client GetByUrl(string url)
        {
            var domainResponse = "";
            return GetByUrl(url, out domainResponse);
        }
        public Client GetByUrl(string url, out string domain)
        {

            var startAddressIndex = url.IndexOf("//") + 2;
            domain = url.Substring(startAddressIndex, url.Substring(startAddressIndex).IndexOf("/"));
            domain = domain.IndexOf("www") == -1 ? "www." + domain : domain;

            return GetByAddress(domain);
        }

        //public Client GetClientWithCatalogId(Catalog_Address catalog)
        //{
        //    return NHibernateSession.Current.CreateCriteria(typeof(Client))
        //        .List<Client>().SingleOrDefault(x => x.catalog.Contains(catalog));
        //}
    }
}
