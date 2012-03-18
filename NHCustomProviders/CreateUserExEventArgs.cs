using System;

namespace NHCustomProviders
{
	/// <summary>Events with information for a new the creation of a new user.</summary>
	public class CreateUserExEventArgs : EventArgs
	{
		#region Fields

		private readonly string _encodedPassword;
		private readonly string _encodedPasswordSalt;
		private readonly string _encodedAnswer;

		#endregion

		#region Properties

		/// <summary>Gets the encoded password for the new user.</summary>
		public string EncodedPassword
		{
			get { return _encodedPassword; }
		}

		/// <summary>Gets the encoded password salt for the new user.</summary>
		public string EncodedPasswordSalt
		{
			get { return _encodedPasswordSalt; }
		}

		/// <summary>Gets the encoded answer for the new user.</summary>
		public string EncodedAnswer
		{
			get { return _encodedAnswer; }
		}

		#endregion

		#region Methods

		/// <summary>Initializes a new instance of the CreateUserExEventArgs class.</summary>
		/// <param name="encodedPassword">The encoded password.</param>
		/// <param name="encodedPasswordSalt">The encoded password salt.</param>
		/// <param name="encodedAnswer">The encoded answer.</param>
		public CreateUserExEventArgs(string encodedPassword, string encodedPasswordSalt, string encodedAnswer)
		{
			_encodedPassword = encodedPassword;
			_encodedPasswordSalt = encodedPasswordSalt;
			_encodedAnswer = encodedAnswer;
		}

		#endregion
	}
}