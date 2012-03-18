using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Web.Security;

namespace NHCustomProviders
{
	/// <summary>Class with all the membership details for an user.</summary>
	public class MembershipUserInfo
	{
		#region Fields

		private bool _isApproved = true;

		#endregion

		#region Properties

		/// <summary>Gets or sets the identifier for the user.</summary>
		public virtual object UserId { get; set; }

		/// <summary>Gets or sets the logon name of the user.</summary>
		public virtual string UserName { get; set; }

		/// <summary>Gets or sets the email address for the user.</summary>
		public virtual string Email { get; set; }

		/// <summary>Gets or sets the password.</summary>
		public virtual string Password { get; set; }

		/// <summary>Gets or sets the password salt.</summary>
		public virtual string PasswordSalt { get; set; }

		/// <summary>Gets or sets the password format.</summary>
		public virtual MembershipPasswordFormat PasswordFormat { get; set; }

		/// <summary>Gets or sets the password question.</summary>
		public virtual string PasswordQuestion { get; set; }

		/// <summary>Gets or sets the password answer.</summary>
		public virtual string PasswordAnswer { get; set; }

		/// <summary>Gets or sets the number of failed attemps to validate the password.</summary>
		public virtual int FailedPasswordAttemptCount { get; set; }

		/// <summary>Gets or sets the date of the first failed attemps when validating the password.</summary>
		public virtual DateTime? FailedPasswordAttemptWindowStart { get; set; }

		/// <summary>Gets or sets the number of failed attemps to validate the password answer.</summary>
		public virtual int FailedPasswordAnswerAttemptCount { get; set; }

		/// <summary>Gets or sets the date of the first failed attemps when validating the password answer.</summary>
		public virtual DateTime? FailedPasswordAnswerAttemptWindowStart { get; set; }

		/// <summary>Gets or sets the date of the last password change.</summary>
		public virtual DateTime? LastPasswordChangedDate { get; set; }

		/// <summary>Gets or sets the date of creation for the user.</summary>
		public virtual DateTime CreationDate { get; set; }

		/// <summary>Gets or sets the last date where the user performed some activity.</summary>
		public virtual DateTime LastActivityDate { get; set; }

		/// <summary>Gets or sets if the user is approved.</summary>
		public virtual bool IsApproved
		{
			get { return _isApproved; }
			set { _isApproved = value; }
		}

		/// <summary>Gets or sets if the user is locked out of the system.</summary>
		public virtual bool IsLockedOut { get; set; }

		/// <summary>Gets or sets the last date when the user was lock out of the system.</summary>
		public virtual DateTime? LastLockOutDate { get; set; }

		/// <summary>Gets or sets the last date when the user logged on the system.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login", Justification = "This matches the MembershipUser property with the same name")]
		public virtual DateTime? LastLoginDate { get; set; }

		/// <summary>Gets or sets comments about the user.</summary>
		public virtual string Comments { get; set; }

		#endregion

		#region Methods

		/// <summary>Initializes a new instance of the MembershipUserInfo class.</summary>
		public MembershipUserInfo()
		{
		}

		/// <summary>Creates a MembershipUser instance based on the current values.</summary>
		/// <param name="providerName">Name of the provider for the instance.</param>
		/// <returns>A new MembershipUser with the current values.</returns>
		public virtual MembershipUser CreateMembershipUser(string providerName)
		{
			return new MembershipUser(providerName, UserName, UserId, Email,
			                          PasswordQuestion, Comments, IsApproved, IsLockedOut, CreationDate,
			                          LastLoginDate ?? DateTime.MinValue, LastActivityDate,
			                          LastPasswordChangedDate ?? DateTime.MinValue, LastLockOutDate ?? DateTime.MinValue);
		}

		/// <summary>Modifies the instance from data obtained from a MembershipUser instance.</summary>
		/// <param name="user">The MembershipUser.</param>
		public virtual void ModifyFromMembershipUser(MembershipUser user)
		{
			UserId = user.ProviderUserKey;
			UserName = user.UserName;
			Email = user.Email;
			PasswordQuestion = user.PasswordQuestion;
			LastPasswordChangedDate = user.LastPasswordChangedDate;
			CreationDate = user.CreationDate;
			LastActivityDate = user.LastActivityDate;
			IsApproved = user.IsApproved;
			IsLockedOut = user.IsLockedOut;
			LastLockOutDate = user.LastLockoutDate;
			LastLoginDate = user.LastLoginDate;
			Comments = user.Comment;
		}

		/// <summary>Determines whether the specified System.Object is equal to the current System.Object</summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(this, obj)) {
				return true;
			}

			MembershipUserInfo user = obj as MembershipUserInfo;
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

		/// <summary>Returns a string that represents the current object.</summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			return String.Format(CultureInfo.CurrentCulture, "[{0}] {1}", (UserId != null) ? UserId.ToString() : "null", UserName ?? "null");
		}

		/// <summary>Implements the operator ==.</summary>
		/// <param name="obj1">The first object to compare.</param>
		/// <param name="obj2">The second object to compare.</param>
		/// <returns>The result of the operator.</returns>
		public static bool operator ==(MembershipUserInfo obj1, MembershipUserInfo obj2)
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
		public static bool operator !=(MembershipUserInfo obj1, MembershipUserInfo obj2)
		{
			return !(obj1 == obj2);
		}

		#endregion
	}
}