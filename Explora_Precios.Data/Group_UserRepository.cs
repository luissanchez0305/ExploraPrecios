using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpArch.Data.NHibernate;
using Explora_Precios.Core.DataInterfaces;
using Explora_Precios.Core;
using NHibernate.Criterion;

namespace Explora_Precios.Data
{
    public class Group_UserRepository : Repository<Group_User>, IGroup_UserRepository
    {
        public IList<Group_User> GetByUser(User user)
        {
            return NHibernateSession.Current.CreateCriteria(typeof(Group_User))
                .Add(Expression.Eq("user", user))
                .List<Group_User>();
        }

        public void Update(Group_User obj)
        {
            using (NHibernate.ISession session = SharpArch.Data.NHibernate.NHibernateSession.Current)
            {
                using (NHibernate.ITransaction transaction = session.BeginTransaction())
                {
                    session.SaveOrUpdate(obj);
                    transaction.Commit();
                }
            }
        }
    }
}
