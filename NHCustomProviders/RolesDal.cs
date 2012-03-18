using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NHibernate;
using NHibernate.Criterion;

namespace NHCustomProviders
{
	/// <summary>Data Access Layer for the custom role provider.</summary>
	public class RolesDal
	{
		#region Properties

		/// <summary>Gets or sets the session to use.</summary>
		public ISession Session { get; set; }

		#endregion

		#region Methods

		/// <summary>Initializes a new instance of the RolesDal class.</summary>
		/// <param name="session">The session.</param>
		public RolesDal(ISession session)
		{
			Session = session;
		}

		/// <summary>Gets a Role by name.</summary>
		/// <param name="roleName">The name of the role.</param>
		/// <returns>The retrieved BasicRole instance or null if it wasn't found.</returns>
		public virtual BasicRole GetRoleByName(string roleName)
		{
			ICriteria criteria = Session.CreateCriteria(typeof(BasicRole));
			criteria.Add(Expression.Eq("RoleName", roleName));
			return criteria.UniqueResult<BasicRole>();
		}

		/// <summary>Gets an User instance by name.</summary>
		/// <param name="userName">Name of the user to retrieve.</param>
		/// <returns>The retrieved BasicUser instance or null if it wasn't found.</returns>
		public virtual BasicUser GetUserByName(string userName)
		{
			ICriteria criteria = Session.CreateCriteria(typeof(BasicUser));
			criteria.Add(Expression.Eq("UserName", userName));
			return criteria.UniqueResult<BasicUser>();
		}

		/// <summary>Deletes the specified role.</summary>
		/// <param name="role">The instance to delete.</param>
		/// <returns>true if the instance was removed from the DB; false otherwise.</returns>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Here we are only interested in knowing if the data was deleted or not")]
		public virtual bool Delete(BasicRole role)
		{
			try {
				Session.Delete(role);

				return true;
			} catch {
				return false;
			}
		}

		/// <summary>Gets all available roles.</summary>
		/// <returns>An array of all roles.</returns>
		public virtual string[] GetAllRoles()
		{
			ICriteria criteria = Session.CreateCriteria(typeof(BasicRole));
			criteria.SetProjection(Projections.Property("RoleName"));
			IList<string> roleNames = criteria.List<string>();
			string[] roles = new string[roleNames.Count];
			roleNames.CopyTo(roles, 0);

			return roles;
		}

		/// <summary>Finds the list of users that belong to a role.</summary>
		/// <param name="roleName">Name of the role.</param>
		/// <param name="userName">Name of the user.</param>
		/// <returns>An array of users that belong to that role.</returns>
		public virtual string[] FindUsersInRole(string roleName, string userName)
		{
			IQuery query = Session.CreateQuery("Select u.UserName FROM BasicUser u INNER JOIN u.Roles r WHERE (u.UserName LIKE :userName) AND (r.RoleName = :roleName) ORDER BY u.UserName");
			query.SetParameter("roleName", roleName);
			query.SetParameter("userName", userName);
			IList<string> userNames = query.List<string>();
			string[] users = new string[userNames.Count];
			userNames.CopyTo(users, 0);

			return users;
		}

		/// <summary>Gets a list of BasicRole instances based on the role names.</summary>
		/// <param name="roleNames">The role names.</param>
		/// <returns>A list of BasicRole instances.</returns>
		public IList<BasicRole> GetRoles(string[] roleNames)
		{
			ICriteria criteria = Session.CreateCriteria(typeof(BasicRole));
			criteria.Add(Expression.In("RoleName", roleNames));
			return criteria.List<BasicRole>();
		}

		#endregion
	}
}