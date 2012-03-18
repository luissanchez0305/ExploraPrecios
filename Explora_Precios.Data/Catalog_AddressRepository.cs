using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Explora_Precios.Core;
using SharpArch.Data.NHibernate;
using Explora_Precios.Core.DataInterfaces;
using NHibernate.Criterion;

namespace Explora_Precios.Data
{
    public class Catalog_AddressRepository : Repository<Catalog_Address>, ICatalog_AddressRepository
    {
        public IList<Catalog_Address> GetByClient(Client client)
        {
            return NHibernateSession.Current.CreateCriteria(typeof(Catalog_Address))
                    .Add(Expression.Eq("client", client))
                    .List<Catalog_Address>();
        }

        public IList<Catalog_Address> GetByClientCatalogDetails(Client client, int level_Id, int catalog_Id)
        {
            var dictionaryRestriction = new Dictionary<string, object>();
            dictionaryRestriction.Add("client", client);
            dictionaryRestriction.Add("level_Id", level_Id);
            dictionaryRestriction.Add("catalog_Id", catalog_Id);
            var response = NHibernateSession.Current.CreateCriteria(typeof(Catalog_Address))
                .Add(Expression.AllEq(dictionaryRestriction))
                .List<Catalog_Address>();
            return response;
        }

        public void DeleteCatalogItem(int item_Id)
        {
            using (NHibernate.ISession session = SharpArch.Data.NHibernate.NHibernateSession.Current)
            {
                using (NHibernate.ITransaction transaction = session.BeginTransaction())
                {
                    SharpArch.Core.PersistenceSupport.IRepository<Catalog_Address> catAdd = new Catalog_AddressRepository();

                    session.Delete(catAdd.Get(item_Id));
                    transaction.Commit();
                }
            }
        }
    }
}
