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
    public class UserRepository : Repository<User>, IUserRepository
    {
        public User GetByEmail(string email)
        {
            return NHibernateSession.Current.CreateCriteria(typeof(User))
                .Add(Expression.Eq("email", email.ToLower()))
                .UniqueResult<User>();
        }

        public void UpdateUser(User user)
        {            
            using (NHibernate.ISession session = SharpArch.Data.NHibernate.NHibernateSession.Current)
            {
                using (NHibernate.ITransaction transaction = session.BeginTransaction())
                {
                    SharpArch.Core.PersistenceSupport.IRepository<User> update = new UserRepository();
                    
                    session.Update(user);
                    transaction.Commit();
                }
            }
        }
    }
}
