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
    public class User_ProductRepository : Repository<User_Product>, IUser_ProductRepository
    {
        public User_Product GetByUserAndProductAndType(User user, Product product, User_Product.RelationType type)
        {
            return NHibernateSession.Current.CreateCriteria(typeof(User_Product))
                .Add(Expression.Conjunction().Add(Expression.Eq("user", user)).Add(Expression.Eq("product", product)).Add(Expression.Eq("Type", type)))
                .UniqueResult<User_Product>();
        }

        public User_Product GetByProductAndUser(Product product, User user)
        {
            return NHibernateSession.Current.CreateCriteria(typeof(User_Product))
                .Add(Expression.Conjunction().Add(Expression.Eq("user", user)).Add(Expression.Eq("product", product)))
                .UniqueResult<User_Product>();
        }

        public IList<User_Product> GetByProductAndActive(Product product)
        {
            return NHibernateSession.Current.CreateCriteria(typeof(User_Product))
                .Add(Expression.And(Expression.Eq("product", product), Expression.Eq("value", float.Parse("1"))))
                .List<User_Product>();
        }

        public void Update(IEnumerable<User_Product> UserProducts)
        {
            using (NHibernate.ISession session = SharpArch.Data.NHibernate.NHibernateSession.Current)
            {
                using (NHibernate.ITransaction transaction = session.BeginTransaction())
                {
                    SharpArch.Core.PersistenceSupport.IRepository<User_Product> update = new User_ProductRepository();

                    foreach (var Follow in UserProducts)
                    {
                        if (Follow.Id > 0)
                            session.Update(Follow);
                    }
                    transaction.Commit();
                }
            }
        }

        public void Update(User_Product FollowOffer, User_Product FollowPrice)
        {
            using (NHibernate.ISession session = SharpArch.Data.NHibernate.NHibernateSession.Current)
            {
                using (NHibernate.ITransaction transaction = session.BeginTransaction())
                {
                    SharpArch.Core.PersistenceSupport.IRepository<User_Product> update = new User_ProductRepository();

                    if (FollowOffer.Id > 0)
                        session.Update(FollowOffer);
                    else if(FollowOffer.value == 1)
                        session.Save(FollowOffer);
                    if (FollowPrice.Id > 0)
                        session.Update(FollowPrice);
                    else if (FollowPrice.value == 1)
                        session.Save(FollowPrice);
                    transaction.Commit();
                }
            }
        }
    }
}
