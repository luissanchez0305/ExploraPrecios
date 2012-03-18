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
    public class DepartmentRepository : Repository<Department>, IDepartmentRepository
    {
        public void DeleteDepartmentItem(int id)
        {
            using (NHibernate.ISession session = SharpArch.Data.NHibernate.NHibernateSession.Current)
            {
                using (NHibernate.ITransaction transaction = session.BeginTransaction())
                {
                    SharpArch.Core.PersistenceSupport.IRepository<Department> del = new DepartmentRepository();

                    session.Delete(del.Get(id));
                    transaction.Commit();
                }
            }
        }

        public Department getByCategory(Category category)
        {
            return NHibernateSession.Current.CreateCriteria(typeof(Category))
                .Add(Expression.Eq("Id", category.Id))
                .UniqueResult<Category>().department;
        }
    }
}
