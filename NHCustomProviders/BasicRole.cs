using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Iesi.Collections.Generic;

namespace NHCustomProviders
{
	/// <summary>Class with the basic details of a role for the Role provider.</summary>
	public class BasicRole
	{
		#region Fields

		private ISet<BasicUser> _users = new HashedSet<BasicUser>();

		#endregion

		#region Properties

		/// <summary>Gets or sets the identifier for the role.</summary>
		public virtual object RoleId { get; set; }

		/// <summary>Gets or sets the name of the role.</summary>
		public virtual string RoleName { get; set; }

		/// <summary>Gets or sets the users for this role.</summary>
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "NHibernate can access the field directly but that will prevent this assembly from running under medium trust")]
		public virtual ISet<BasicUser> Users
		{
			get { return _users; }
			set { _users = value; }
		}

		#endregion

		#region Methods

		/// <summary>Initializes a new instance of the BasicUser class.</summary>
		public BasicRole()
		{
		}

		/// <summary>Determines whether the specified System.Object is equal to the current System.Object</summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) {
				return true;
			}

			BasicRole role = obj as BasicRole;
			if (role == null) {
				return false;
			}

			if (RoleName != role.RoleName) {
				return false;
			}

			return true;
		}

		/// <summary>Serves as a hash function for a particular type. GetHashCode is suitable for use in hashing algorithms and data structures like a hash table.</summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			int result = 0;

			if (RoleName != null) {
				result += RoleName.GetHashCode();
			}

			return result;
		}

		/// <summary>Implements the operator ==.</summary>
		/// <param name="obj1">The first object to compare.</param>
		/// <param name="obj2">The second object to compare.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator ==(BasicRole obj1, BasicRole obj2)
		{
			// if both are null, or both are same instance, return true.
			if (ReferenceEquals(obj1, obj2)) {
				return true;
			}

			// if one is null, but not both, return false.
			if (((object)obj1 == null) || ((object)obj2 == null)) {
				return false;
			}

			// use the equals method
			return obj1.Equals(obj2);
		}

		/// <summary>Implements the operator !=.</summary>
		/// <param name="obj1">The first object to compare.</param>
		/// <param name="obj2">The second object to compare.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator !=(BasicRole obj1, BasicRole obj2)
		{
			return !(obj1 == obj2);
		}

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return String.Format(CultureInfo.CurrentCulture, "[{0}] {1}", (RoleId != null) ? RoleId.ToString() : "null", RoleName ?? "null");
		}

		#endregion
	}
}