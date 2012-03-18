using System;
using System.Diagnostics.CodeAnalysis;

namespace NHCustomProviders
{
	/// <summary>Class that contains the information to map the Membership provider.</summary>
	public class MembershipClassMappingInfo
	{
		#region Properties

		/// <summary>Name of the table mapped associated to the class.</summary>
		public string TableName { get; set; }

		/// <summary>If dynamic update is used or not.</summary>
		public bool DynamicUpdate { get; set; }

		/// <summary>Column name of the id column.</summary>
		public string IdColumnName { get; set; }

		/// <summary>Type of the id column.</summary>
		public string IdColumnType { get; set; }

		/// <summary>The generator strategy for the id.</summary>
		public string GeneratorStrategy { get; set; }

		/// <summary>Column name for the UserName property.</summary>
		public string UserNameColumn { get; set; }

		/// <summary>Indicates the length of the UserName.</summary>
		public int UserNameColumnSize { get; set; }

		/// <summary>Column name for the Email property.</summary>
		public string EmailColumn { get; set; }

		/// <summary>Indicates the length of the Email.</summary>
		public int EmailColumnSize { get; set; }

		/// <summary>Indicates if the Email property is unique.</summary>
		public bool UniqueEmail { get; set; }

		/// <summary>Column name for the Password property.</summary>
		public string PasswordColumn { get; set; }

		/// <summary>Indicates the length of the Password.</summary>
		public int PasswordColumnSize { get; set; }

		/// <summary>Column name for the PasswordSalt property.</summary>
		public string PasswordSaltColumn { get; set; }

		/// <summary>Indicates for the length of the PasswordSalt.</summary>
		public int PasswordSaltColumnSize { get; set; }

		/// <summary>Column name for the PasswordFormat property.</summary>
		public string PasswordFormatColumn { get; set; }

		/// <summary>Column name for the PasswordQuestion property.</summary>
		public string PasswordQuestionColumn { get; set; }

		/// <summary>Indicates the length of the PasswordQuestion.</summary>
		public int PasswordQuestionColumnSize { get; set; }

		/// <summary>Column name for the PasswordAnswer property.</summary>
		public string PasswordAnswerColumn { get; set; }

		/// <summary>Indicates the length of the PasswordAnswer.</summary>
		public int PasswordAnswerColumnSize { get; set; }

		/// <summary>Column name for the FailedPasswordAttemptCount property.</summary>
		public string FailedPasswordAttemptCountColumn { get; set; }

		/// <summary>Column name for the FailedPasswordWindowStart property.</summary>
		public string FailedPasswordAttemptWindowStartColumn { get; set; }

		/// <summary>Column name for the FailedPasswordAnswerAttemptCount property.</summary>
		public string FailedPasswordAnswerAttemptCountColumn { get; set; }

		/// <summary>Column name for the FailedPasswordAnswerAttemptWindowStart property.</summary>
		public string FailedPasswordAnswerAttemptWindowStartColumn { get; set; }

		/// <summary>Column name for the LastPasswordChangedDate property.</summary>
		public string LastPasswordChangedDateColumn { get; set; }

		/// <summary>Column name for the CreationDate property.</summary>
		public string CreationDateColumn { get; set; }

		/// <summary>Column name for the LastActivityDate property.</summary>
		public string LastActivityDateColumn { get; set; }

		/// <summary>Column name for the IsApproved property.</summary>
		public string IsApprovedColumn { get; set; }

		/// <summary>Column name for the IsLockedOut property.</summary>
		public string IsLockedOutColumn { get; set; }

		/// <summary>Column name for the LastLockOutDate property.</summary>
		public string LastLockOutDateColumn { get; set; }

		/// <summary>Column name for the LastLoginDate property.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login")]
		public string LastLoginDateColumn { get; set; }

		/// <summary>Column name for the Comments property.</summary>
		public string CommentsColumn { get; set; }

		#endregion
	}
}