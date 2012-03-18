using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Iesi.Collections.Generic;

namespace NHCustomProviders
{
	/// <summary>Class with the basic details of an user for the Role provider.</summary>
	public class BasicUser
	{
		#region Fields

		private ISet<BasicRole> _roles = new HashedSet<BasicRole>();

		#endregion

		#region Properties

		/// <summary>Gets or sets the identifier for the user.</summary>
		public virtual object UserId { get; set; }

		/// <summary>Gets or sets the logon name of the user.</summary>
		public virtual string UserName { get; set; }

		/// <summary>Gets or sets the roles for this user.</summary>
		[SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "NHibernate can access the field directly but that will prevent this assembly from running under medium trust")]
		public virtual ISet<BasicRole> Roles
		{
			get { return _roles; }
			set { _roles = value; }
		}

		#endregion

		#region Methods

		/// <summary>Initializes a new instance of the BasicUser class.</summary>
		public BasicUser()
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

			BasicUser user = obj as BasicUser;
			if (user == null) {
				return false;
			}

			if (UserName != user.UserName) {
				return false;
			}

			return true;
		}

		/// <summary>Serves as a hash function for a particular type. GetHashCode is suitable for use in hashing algorithms and data structures like a hash table.</summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			int result = 0;

			if (UserName != null) {
				result += UserName.GetHashCode();
			}

			return result;
		}

		/// <summary>Implements the operator ==.</summary>
		/// <param name="obj1">The first object to compare.</param>
		/// <param name="obj2">The second object to compare.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator ==(BasicUser obj1, BasicUser obj2)
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
		public static bool operator !=(BasicUser obj1, BasicUser obj2)
		{
			return !(obj1 == obj2);
		}

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return String.Format(CultureInfo.CurrentCulture, "[{0}] {1}", (UserId != null) ? UserId.ToString() : "null", UserName ?? "null");
		}

		#endregion
	}
}