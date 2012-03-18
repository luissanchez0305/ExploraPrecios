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
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public IList<Category> GetByDepartament(Department departament)
        {
            return NHibernateSession.Current.CreateCriteria(typeof(Category))
                .Add(Expression.Eq("department", departament))
                .List<Category>();
        }

        public Category GetBySubCategory(SubCategory subCategory)
        {
            return NHibernateSession.Current.CreateCriteria(typeof(SubCategory))
                .Add(Expression.Eq("Id", subCategory.Id))
                .UniqueResult<SubCategory>().category;
        }

        //public List<Category> GetByDepartamentName(string depName)
        //{
        //    return NHibernateSession.Current.CreateCriteria(typeof(Category))
        //        .List<Category>().Where(x => x.department.name == depName).ToList();
        //}

        public void DeleteCategoryItem(int id)
        {
            using (NHibernate.ISession session = SharpArch.Data.NHibernate.NHibernateSession.Current)
            {
                using (NHibernate.ITransaction transaction = session.BeginTransaction())
                {
                    SharpArch.Core.PersistenceSupport.IRepository<Category> del = new CategoryRepository();

                    session.Delete(del.Get(id));
                    transaction.Commit();
                }
            }
        }
    }
}
