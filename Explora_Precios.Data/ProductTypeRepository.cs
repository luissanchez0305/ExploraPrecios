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
    public class ProductTypeRepository : Repository<ProductType>, IProductTypeRepository
    {
        public IList<ProductType> GetBySubCategory(SubCategory subCategory)
        {
            return NHibernateSession.Current.CreateCriteria(typeof(ProductType))
                .Add(Expression.Eq("subCategory", subCategory))
                .List<ProductType>();
        }

        public void DeleteProductTypeItem(int id)
        {
            using (NHibernate.ISession session = SharpArch.Data.NHibernate.NHibernateSession.Current)
            {
                using (NHibernate.ITransaction transaction = session.BeginTransaction())
                {
                    SharpArch.Core.PersistenceSupport.IRepository<ProductType> del = new ProductTypeRepository();

                    session.Delete(del.Get(id));
                    transaction.Commit();
                }
            }
        }
    }
}
