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
    public class SubCategoryRepository : Repository<SubCategory>, ISubCategoryRepository
    {
        public IList<SubCategory> GetByCategory(Category category)
        {
            return NHibernateSession.Current.CreateCriteria(typeof(SubCategory))
                .Add(Expression.Eq("category", category)).List<SubCategory>();
        }

        public SubCategory GetByProductType(ProductType productType)
        {
            return NHibernateSession.Current.CreateCriteria(typeof(ProductType))
                .Add(Expression.Eq("Id", productType.Id))
                .UniqueResult<ProductType>().subCategory;
        }

        public void DeleteSubCategoryItem(int id)
        {
            using (NHibernate.ISession session = SharpArch.Data.NHibernate.NHibernateSession.Current)
            {
                using (NHibernate.ITransaction transaction = session.BeginTransaction())
                {
                    SharpArch.Core.PersistenceSupport.IRepository<SubCategory> del = new SubCategoryRepository();

                    session.Delete(del.Get(id));
                    transaction.Commit();
                }
            }
        }
    }
}
