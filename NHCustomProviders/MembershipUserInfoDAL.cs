using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NHibernate;
using NHibernate.Criterion;

namespace NHCustomProviders
{
	/// <summary>Data Access Layer for the MembershipUserInfo class.</summary>
	public class MembershipUserInfoDal
	{
		#region Properties

		/// <summary>Gets or sets the session to use.</summary>
		public ISession Session { get; set; }

		#endregion

		#region Methods

		/// <summary>Initializes a new instance of the MembershipUserInfoDal class.</summary>
		/// <param name="session">The session.</param>
		public MembershipUserInfoDal(ISession session)
		{
			Session = session;
		}

		/// <summary>Gets a MembershipUserInfo by identifier.</summary>
		/// <param name="id">The id.</param>
		/// <returns>The retrieved MembershipUserInfo instance or null if it wasn't found.</returns>
		public virtual MembershipUserInfo GetById(object id)
		{
			return Session.Get<MembershipUserInfo>(id);
		}

		/// <summary>Gets a MembershipUserInfo instance by the user name.</summary>
		/// <param name="userName">Name of the user to retrieve.</param>
		/// <returns>The retrieved MembershipUserInfo instance or null if it wasn't found.</returns>
		public virtual MembershipUserInfo GetByUserName(string userName)
		{
			ICriteria criteria = Session.CreateCriteria(typeof(MembershipUserInfo));
			criteria.Add(Expression.Eq("UserName", userName));
			return criteria.UniqueResult<MembershipUserInfo>();
		}

		/// <summary>Gets a MembershipUserInfo instance by the email.</summary>
		/// <param name="email">Email of the user to retrieve.</param>
		/// <returns>The retrieved MembershipUserInfo instance or null if it wasn't found.</returns>
		public virtual IList GetUserNameByEmail(string email)
		{
			ICriteria criteria = Session.CreateCriteria(typeof(MembershipUserInfo));
			criteria.Add(Expression.Eq("Email", email).IgnoreCase());
			criteria.SetProjection(Projections.Property("UserName"));
			return criteria.List();
		}

		/// <summary>Deletes the specified MembershipUserInfo.</summary>
		/// <param name="mui">The instance to delete.</param>
		/// <returns>true if the instance was removed from the DB; false otherwise.</returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Here we are only interested in knowing if the data was deleted or not")]
		public virtual bool Delete(MembershipUserInfo mui)
		{
			try {
				Session.Delete(mui);

				return true;
			} catch {
				return false;
			}
		}

		/// <summary>Gets all the MembershipUserInfo by email.</summary>
		/// <param name="email">The email of the users to retrieve.</param>
		/// <param name="startIndex">The start index for paging.</param>
		/// <param name="maxRows">The maximum rows for paging.</param>
		/// <param name="numRows">The total number of users matching the criteria.</param>
		/// <returns>An IList of MembershipUserInfo instances.</returns>
		public virtual IList<MembershipUserInfo> GetByEmail(string email, int startIndex, int maxRows, out int numRows)
		{
			ICriteria criteria1 = Session.CreateCriteria(typeof(MembershipUserInfo));
			criteria1.SetProjection(Projections.RowCount());
			criteria1.Add(Expression.Eq("Email", email).IgnoreCase());
			numRows = criteria1.UniqueResult<int>();

			ICriteria criteria2 = Session.CreateCriteria(typeof(MembershipUserInfo));
			criteria2.Add(Expression.Eq("Email", email).IgnoreCase());
			criteria2.SetFirstResult(startIndex);
			criteria2.SetMaxResults(maxRows);
			return criteria2.List<MembershipUserInfo>();
		}

		/// <summary>Gets all the MembershipUserInfo by user name.</summary>
		/// <param name="userName">The user name of the users to retrieve (it doesn't make much sense if there is only one application).</param>
		/// <param name="startIndex">The start index for paging.</param>
		/// <param name="maxRows">The maximum rows for paging.</param>
		/// <param name="numRows">The total number of users matching the criteria.</param>
		/// <returns>An IList of MembershipUserInfo instances.</returns>
		public virtual IList<MembershipUserInfo> GetByUserName(string userName, int startIndex, int maxRows, out int numRows)
		{
			ICriteria criteria1 = Session.CreateCriteria(typeof(MembershipUserInfo));
			criteria1.SetProjection(Projections.RowCount());
			criteria1.Add(Expression.Eq("UserName", userName));
			numRows = criteria1.UniqueResult<int>();

			ICriteria criteria2 = Session.CreateCriteria(typeof(MembershipUserInfo));
			criteria2.Add(Expression.Eq("UserName", userName));
			criteria2.SetFirstResult(startIndex);
			criteria2.SetMaxResults(maxRows);
			return criteria2.List<MembershipUserInfo>();
		}

		/// <summary>Gets all the users from the DB.</summary>
		/// <param name="startIndex">The start index for paging.</param>
		/// <param name="maxRows">The maximum rows for paging.</param>
		/// <param name="numRows">The total number of users matching the criteria.</param>
		/// <returns>An IList of MembershipUserInfo instances.</returns>
		public virtual IList<MembershipUserInfo> GetAll(int startIndex, int maxRows, out int numRows)
		{
			ICriteria criteria1 = Session.CreateCriteria(typeof(MembershipUserInfo));
			criteria1.SetProjection(Projections.RowCount());
			numRows = criteria1.UniqueResult<int>();

			ICriteria criteria2 = Session.CreateCriteria(typeof(MembershipUserInfo));
			criteria2.SetFirstResult(startIndex);
			criteria2.SetMaxResults(maxRows);
			return criteria2.List<MembershipUserInfo>();
		}

		/// <summary>Gets the number of users online.</summary>
		/// <param name="dateActive">A date used to check if the user is online.</param>
		/// <param name="hasLastActivityDate">if set to true, the LastActivityDate property is used to chechk if the user is only. Otherwise, the LastLoginDate is used.</param>
		/// <returns>The number of users online.</returns>
		public virtual int GetNumberOfUsersOnline(DateTime dateActive, bool hasLastActivityDate)
		{
			ICriteria criteria = Session.CreateCriteria(typeof(MembershipUserInfo));
			criteria.SetProjection(Projections.RowCount());
			if (hasLastActivityDate) {
				criteria.Add(Expression.Gt("LastActivityDate", dateActive));
			} else {
				criteria.Add(Expression.Gt("LastLoginDate", dateActive));
			}
			return criteria.UniqueResult<int>();
		}

		#endregion
	}
}