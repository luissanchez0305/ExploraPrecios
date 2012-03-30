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
	public class Group_UserEqualityComparer : IEqualityComparer<Group_User>
	{
		public bool Equals(Group_User x, Group_User y)
		{
			// Compare properties for equality
			return (x.product.Id == y.product.Id);
		}

		public int GetHashCode(Group_User obj)
		{
			return obj.product.Id.GetHashCode();
		}
	}

	public class Group_UserRepository : Repository<Group_User>, IGroup_UserRepository
	{
		public IEnumerable<Group_User> GetLatest()
		{
			return NHibernateSession.Current.CreateCriteria(typeof(Group_User))
				.Add(Expression.Ge("created", DateTime.Now.AddMonths(-3))).AddOrder(Order.Desc("created"))
				.List<Group_User>().AsEnumerable().Distinct(new Group_UserEqualityComparer());

		}

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
