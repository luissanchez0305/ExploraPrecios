//===============================================================================
// NHCustomMembershipProvider
//
//	Copyright (c) 2007-2008 Manuel Abadia (http://www.manuelabadia.com)
//	All rights reserverd
//
//	Redistribution and use in source and binary forms, with or without
//	modification, are permitted provided that the following conditions
//	are met:
//
// 1. Redistributions of source code must retain the above copyright
//    notice, this list of conditions and the following disclaimer.
// 2. Redistributions in binary form must reproduce the above copyright
//    notice, this list of conditions and the following disclaimer in the
//    documentation and/or other materials provided with the distribution.
// 3. The source code can not be used as the base to create a commercial
//    membership and/or role provider.
//
// THIS SOFTWARE IS PROVIDED BY THE AUTHOR "AS IS" AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
// OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
// IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
// INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
// NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
// (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
// THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//	Notes:
//	* user names are case sensitive.
//	* ApplicationName is ignored.
//===============================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Management;
using System.Web.Security;
using System.Xml;
using NHibernate;
using NHibernate.Cfg;

namespace NHCustomProviders
{
	/// <summary>A highly configurable custom membership provider that uses NHibernate for DB access.</summary>
	[AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public class NHCustomMembershipProvider : MembershipProvider
	{
		#region Fields

		private static ISessionFactory _sessionFactory;
		private const int PASSWORD_SIZE = 10;

		private string _applicationName;
		private string _configurationFile;
		private bool _enablePasswordReset;
		private bool _enablePasswordRetrieval;
		private int _maxInvalidPasswordAttempts;
		private int _passwordAttemptWindow;
		private int _minRequiredNonAlphanumericCharacters;
		private int _minRequiredPasswordLength;
		private MembershipPasswordFormat _passwordFormat;
		private string _passwordStrengthRegularExpression;
		private bool _requiresQuestionAndAnswer;
		private bool _requiresUniqueEmail;

		private string _tableName;
		private string _userIdColumn;
		private string _userIdType;
		private string _idGeneratorClass;
		private string _userNameColumn;
		private string _emailColumn;
		private string _passwordColumn;
		private string _passwordSaltColumn;
		private string _passwordFormatColumn;
		private string _passwordQuestionColumn;
		private string _passwordAnswerColumn;
		private string _failedPasswordAttemptCountColumn;
		private string _failedPasswordAttemptWindowStartColumn;
		private string _failedPasswordAnswerAttemptCountColumn;
		private string _failedPasswordAnswerAttemptWindowStartColumn;
		private string _lastPasswordChangedDateColumn;
		private string _creationDateColumn;
		private string _lastActivityDateColumn;
		private string _isApprovedColumn;
		private string _isLockedOutColumn;
		private string _lastLockOutDateColumn;
		private string _lastLoginDateColumn;
		private string _commentsColumn;

		private int _userNameColumnSize;
		private int _emailColumnSize;
		private int _passwordColumnSize;
		private int _passwordSaltColumnSize;
		private int _passwordQuestionColumnSize;
		private int _passwordAnswerColumnSize;

		private int _passwordSaltSize;
		private bool _raiseSystemEvents;
		private bool _dynamicUpdate;

		private bool _enableAutoUnlock;
		private int _autoUnlockMinutes;
		private bool _ignoreCreateUserMethod;

		#endregion

		#region Properties

		/// <summary>Gets the session factory.</summary>
		protected internal static ISessionFactory SessionFactory
		{
			get { return _sessionFactory; }
		}

		/// <summary>The name of the application using the custom membership provider.</summary>
		/// <remarks>This property is not persisted in the DB but maintained for compatibility reasons.</remarks>
		/// <returns>The name of the application using the custom membership provider.</returns>
		public override string ApplicationName
		{
			get { return _applicationName; }
			set { _applicationName = value; }
		}

		/// <summary>Gets a value indicating the file to the configuration file for NHibernate.</summary>
		/// <remarks>By default, the provider uses the configuration in the hibernate-configuration section or in 
		/// the hibernate.cfg.xml file. Using this property you to use a different file for the configuration.</remarks>
		public virtual string ConfigurationFile
		{
			get { return _configurationFile; }
		}

		/// <summary>Indicates whether the membership provider is configured to allow users to reset their passwords.</summary>
		/// <returns>true if the membership provider supports password reset; otherwise, false. The default is true.</returns>
		public override bool EnablePasswordReset
		{
			get { return _enablePasswordReset; }
		}

		/// <summary>Indicates whether the membership provider is configured to allow users to retrieve their passwords.</summary>
		/// <returns>true if the membership provider is configured to support password retrieval; otherwise, false. The default is false.</returns>
		public override bool EnablePasswordRetrieval
		{
			get { return _enablePasswordRetrieval; }
		}

		/// <summary>Gets the number of invalid password or password-answer attempts allowed before the membership user is locked out.</summary>
		/// <returns>The number of invalid password or password-answer attempts allowed before the membership user is locked out.</returns>
		public override int MaxInvalidPasswordAttempts
		{
			get { return _maxInvalidPasswordAttempts; }
		}

		/// <summary>Gets the number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.</summary>
		/// <returns>The number of minutes in which a maximum number of invalid password or password-answer attempts are allowed before the membership user is locked out.</returns>
		public override int PasswordAttemptWindow
		{
			get { return _passwordAttemptWindow; }
		}

		/// <summary>Gets the minimum number of special characters that must be present in a valid password.</summary>
		/// <returns>The minimum number of special characters that must be present in a valid password.</returns>
		public override int MinRequiredNonAlphanumericCharacters
		{
			get { return _minRequiredNonAlphanumericCharacters; }
		}

		/// <summary>Gets the minimum length required for a password.</summary>
		/// <returns>The minimum length required for a password. </returns>
		public override int MinRequiredPasswordLength
		{
			get { return _minRequiredPasswordLength; }
		}

		/// <summary>Gets a value indicating the format for storing passwords in the membership data store.</summary>
		/// <returns>One of the MembershipPasswordFormat values indicating the format for storing passwords in the data store.</returns>
		public override MembershipPasswordFormat PasswordFormat
		{
			get { return _passwordFormat; }
		}

		/// <summary>Gets the regular expression used to evaluate a password.</summary>
		/// <returns>A regular expression used to evaluate a password.</returns>
		public override string PasswordStrengthRegularExpression
		{
			get { return _passwordStrengthRegularExpression; }
		}

		/// <summary>Gets a value indicating whether the membership provider is configured to require the user to answer a password question for password reset and retrieval.</summary>
		/// <returns>true if a password answer is required for password reset and retrieval; otherwise, false. The default is true.</returns>
		public override bool RequiresQuestionAndAnswer
		{
			get { return _requiresQuestionAndAnswer; }
		}

		/// <summary>Gets a value indicating whether the membership provider is configured to require a unique e-mail address for each user name.</summary>
		/// <returns>true if the membership provider requires a unique e-mail address; otherwise, false. The default is true.</returns>
		public override bool RequiresUniqueEmail
		{
			get { return _requiresUniqueEmail; }
		}


		/// <summary>Gets a value indicating whether the last activity date is stored in the DB.</summary>
		public virtual bool HasLastActivityDate
		{
			get { return _lastActivityDateColumn != null; }
		}

		/// <summary>Gets a value indicating whether the last login date is stored in the DB.</summary>
		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login", Justification = "This is done to match the MembershipUser property LastLoginDate")]
		public virtual bool HasLastLoginDate
		{
			get { return _lastLoginDateColumn != null; }
		}

		/// <summary>Gets the size of the password salt.</summary>
		/// <value>The size of the password salt.</value>
		public virtual int PasswordSaltSize
		{
			get { return _passwordSaltSize; }
		}

		/// <summary>Gets a value indicating whether system events are raised.</summary>
		/// <remarks>To raise system events full trust is needed or to run in medium trust, the assembly must be registered in the GAC.</remarks>
		public virtual bool RaiseSystemEvents
		{
			get { return _raiseSystemEvents; }
		}

		/// <summary>Gets a value indicating whether auto unlocking is enabled.</summary>
		public virtual bool EnableAutoUnlock
		{
			get { return _enableAutoUnlock; }
		}

		/// <summary>Gets a value indicating the number of minutes after an auto unlock can be performed.</summary>
		public virtual int AutoUnlockMinutes
		{
			get { return _autoUnlockMinutes; }
		}

		/// <summary>Gets a value indicating whether to ignore the CreateUser method.</summary>
		/// <remarks>This is useful if the user will be created outside the provider and to use the CreateUserExWizard control.</remarks>
		public virtual bool IgnoreCreateUserMethod
		{
			get { return _ignoreCreateUserMethod; }
		}

		#endregion

		#region Methods

		#region Init/End Methods

		/// <summary>Initializes the provider.</summary>
		/// <param name="name">The friendly name of the provider.</param>
		/// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
		[SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
		[SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "System.Text.RegularExpressions.Regex", Justification = "This is a fair use, as we check if the RegEx is valid or not, even if the instance is not used later")]
		public override void Initialize(string name, NameValueCollection config)
		{
			if (config == null) {
				throw new ArgumentNullException("config");
			}

			if (String.IsNullOrEmpty(name)) {
				name = "NHCustomMembershipProvider";
			}
			if (String.IsNullOrEmpty(config["description"])) {
				config.Add("description", "NHibernate Custom Membership Provider (c) Manuel Abadia 2007");
			}

			base.Initialize(name, config);

			// try to get the provider configuration values

			_applicationName = config["applicationName"];
			config.Remove("applicationName");

			_configurationFile = config["configurationFile"];
			config.Remove("configurationFile");

			_enablePasswordReset = ConfigurationHelper.GetBoolean(config, "enablePasswordReset", true);
			config.Remove("enablePasswordReset");

			_enablePasswordRetrieval = ConfigurationHelper.GetBoolean(config, "enablePasswordRetrieval", true);
			config.Remove("enablePasswordRetrieval");

			_minRequiredNonAlphanumericCharacters = ConfigurationHelper.GetInt32(config, "minRequiredNonAlphanumericCharacters", 1, 0, Int32.MaxValue);
			config.Remove("minRequiredNonAlphanumericCharacters");

			_minRequiredPasswordLength = ConfigurationHelper.GetInt32(config, "minRequiredPasswordLength", 6, 1, Int32.MaxValue);
			config.Remove("minRequiredPasswordLength");

			if (_minRequiredNonAlphanumericCharacters > _minRequiredPasswordLength) {
				throw new ProviderException("Error, the number of minimum required non alphanumeric characters can't be greater than the minimum required length");
			}

			_passwordFormat = (MembershipPasswordFormat)ConfigurationHelper.GetEnum(config, "passwordFormat", typeof(MembershipPasswordFormat), (int)MembershipPasswordFormat.Clear);
			config.Remove("passwordFormat");

			// check if password retrieval is possible using the specified password format
			if (_enablePasswordRetrieval && (_passwordFormat == MembershipPasswordFormat.Hashed)) {
				throw new ProviderException("Error, the Hashed password format does not allow password retrieval");
			}

			_passwordStrengthRegularExpression = ConfigurationHelper.GetString(config, "passwordStrengthRegularExpression", null);

			// if there is regular expression to check password strength, check that is a valid regular expression
			if (!String.IsNullOrEmpty(_passwordStrengthRegularExpression)) {
				try {
					new Regex(_passwordStrengthRegularExpression);
				} catch (ArgumentException e) {
					throw new ProviderException(e.Message, e);
				}
			}

			config.Remove("passwordStrengthRegularExpression");

			_maxInvalidPasswordAttempts = ConfigurationHelper.GetInt32(config, "maxInvalidPasswordAttempts", 5, 1, Int32.MaxValue);
			config.Remove("maxInvalidPasswordAttempts");

			_passwordAttemptWindow = ConfigurationHelper.GetInt32(config, "passwordAttemptWindow", 10, 1, Int32.MaxValue);
			config.Remove("passwordAttemptWindow");

			_requiresQuestionAndAnswer = ConfigurationHelper.GetBoolean(config, "requiresQuestionAndAnswer", true);
			config.Remove("requiresQuestionAndAnswer");

			_requiresUniqueEmail = ConfigurationHelper.GetBoolean(config, "requiresUniqueEmail", false);
			config.Remove("requiresUniqueEmail");


			// try to get the table and column names for the mapping
			_tableName = ConfigurationHelper.GetString(config, "tableName", null);
			config.Remove("tableName");

			if (String.IsNullOrEmpty(_tableName)) {
				throw new ProviderException("Error, the tableName property must be provided");
			}

			_userIdColumn = ConfigurationHelper.GetString(config, "userIdColumn", null);
			config.Remove("userIdColumn");

			if (String.IsNullOrEmpty(_userIdColumn)) {
				throw new ProviderException("Error, the userIdColumn property must be provided");
			}

			_userIdType = ConfigurationHelper.GetString(config, "userIdType", "Int32");
			config.Remove("userIdType");

			if (String.IsNullOrEmpty(_userIdType)) {
				throw new ProviderException("Error, the userIdType property must be provided");
			}

			_idGeneratorClass = ConfigurationHelper.GetString(config, "idGeneratorClass", "native");
			config.Remove("idGeneratorClass");

			_userNameColumn = ConfigurationHelper.GetString(config, "userNameColumn", null);
			config.Remove("userNameColumn");

			if (String.IsNullOrEmpty(_userNameColumn)) {
				throw new ProviderException("Error, the userNameColumn property must be provided");
			}

			_emailColumn = ConfigurationHelper.GetString(config, "emailColumn", null);
			config.Remove("emailColumn");

			if (String.IsNullOrEmpty(_emailColumn)) {
				throw new ProviderException("Error, the emailColumn property must be provided");
			}

			_passwordColumn = ConfigurationHelper.GetString(config, "passwordColumn", null);
			config.Remove("passwordColumn");

			if (String.IsNullOrEmpty(_passwordColumn)) {
				throw new ProviderException("Error, the passwordColumn property must be provided");
			}

			_passwordSaltColumn = ConfigurationHelper.GetString(config, "passwordSaltColumn", null);
			config.Remove("passwordSaltColumn");

			_passwordFormatColumn = ConfigurationHelper.GetString(config, "passwordFormatColumn", null);
			config.Remove("passwordFormatColumn");

			_passwordQuestionColumn = ConfigurationHelper.GetString(config, "passwordQuestionColumn", null);
			config.Remove("passwordQuestionColumn");

			if (String.IsNullOrEmpty(_passwordQuestionColumn) && _requiresQuestionAndAnswer) {
				throw new ProviderException("Error, the passwordQuestionColumn property must be provided");
			}

			_passwordAnswerColumn = ConfigurationHelper.GetString(config, "passwordAnswerColumn", null);
			config.Remove("passwordAnswerColumn");

			if (String.IsNullOrEmpty(_passwordAnswerColumn) && _requiresQuestionAndAnswer) {
				throw new ProviderException("Error, the passwordAnswerColumn property must be provided");
			}

			_failedPasswordAttemptCountColumn = ConfigurationHelper.GetString(config, "failedPasswordAttemptCountColumn", null);
			config.Remove("failedPasswordAttemptCountColumn");

			_failedPasswordAttemptWindowStartColumn = ConfigurationHelper.GetString(config, "failedPasswordAttemptWindowStartColumn", null);
			config.Remove("failedPasswordAttemptWindowStartColumn");

			_failedPasswordAnswerAttemptCountColumn = ConfigurationHelper.GetString(config, "failedPasswordAnswerAttemptCountColumn", null);
			config.Remove("failedPasswordAnswerAttemptCountColumn");

			_failedPasswordAnswerAttemptWindowStartColumn = ConfigurationHelper.GetString(config, "failedPasswordAnswerAttemptWindowStartColumn", null);
			config.Remove("failedPasswordAnswerAttemptWindowStartColumn");

			_lastPasswordChangedDateColumn = ConfigurationHelper.GetString(config, "lastPasswordChangedDateColumn", null);
			config.Remove("lastPasswordChangedDateColumn");

			_creationDateColumn = ConfigurationHelper.GetString(config, "creationDateColumn", null);
			config.Remove("creationDateColumn");

			_lastActivityDateColumn = ConfigurationHelper.GetString(config, "lastActivityDateColumn", null);
			config.Remove("lastActivityDateColumn");

			_isApprovedColumn = ConfigurationHelper.GetString(config, "isApprovedColumn", null);
			config.Remove("isApprovedColumn");

			_isLockedOutColumn = ConfigurationHelper.GetString(config, "isLockedOutColumn", null);
			config.Remove("isLockedOutColumn");

			// if there is a lockedOut column, there has to be a counter and datetime column to track failed passwords
			if (!String.IsNullOrEmpty(_isLockedOutColumn) &&
			    ((String.IsNullOrEmpty(_failedPasswordAttemptWindowStartColumn) || String.IsNullOrEmpty(_failedPasswordAttemptCountColumn)) &&
			     (String.IsNullOrEmpty(_failedPasswordAnswerAttemptWindowStartColumn) || String.IsNullOrEmpty(_failedPasswordAnswerAttemptCountColumn)))) {
				throw new ProviderException("Error, there is a isLockedOut column but no columns to track failed attempts are present.");
			}

			_lastLockOutDateColumn = ConfigurationHelper.GetString(config, "lastLockOutDateColumn", null);
			config.Remove("lastLockOutDateColumn");

			_lastLoginDateColumn = ConfigurationHelper.GetString(config, "lastLoginDateColumn", null);
			config.Remove("lastLoginDateColumn");

			_commentsColumn = ConfigurationHelper.GetString(config, "commentsColumn", null);
			config.Remove("commentsColumn");

			// try to get the size of the columns
			_userNameColumnSize = ConfigurationHelper.GetInt32(config, "userNameColumnSize", -1, 1, Int32.MaxValue);
			config.Remove("userNameColumnSize");

			_emailColumnSize = ConfigurationHelper.GetInt32(config, "emailColumnSize", -1, 1, Int32.MaxValue);
			config.Remove("emailColumnSize");

			_passwordColumnSize = ConfigurationHelper.GetInt32(config, "passwordColumnSize", -1, 1, Int32.MaxValue);
			config.Remove("passwordColumnSize");

			_passwordSaltColumnSize = ConfigurationHelper.GetInt32(config, "passwordSaltColumnSize", -1, 1, Int32.MaxValue);
			config.Remove("passwordSaltColumnSize");

			_passwordQuestionColumnSize = ConfigurationHelper.GetInt32(config, "passwordQuestionColumnSize", -1, 1, Int32.MaxValue);
			config.Remove("passwordQuestionColumnSize");

			_passwordAnswerColumnSize = ConfigurationHelper.GetInt32(config, "passwordAnswerColumnSize", -1, 1, Int32.MaxValue);
			config.Remove("passwordAnswerColumnSize");

			// get the remaining parameters
			_passwordSaltSize = ConfigurationHelper.GetInt32(config, "passwordSaltSize", 16, 1, Int32.MaxValue);
			config.Remove("passwordSaltSize");

			if ((_passwordSaltColumn != null) && (_passwordSaltSize < 0)) {
				throw new ProviderException("Error, there is a column to store the passworld salt but you haven't specified a valid passwordSaltSize parameter.");
			}

			_raiseSystemEvents = ConfigurationHelper.GetBoolean(config, "raiseSystemEvents", false);
			config.Remove("raiseSystemEvents");

			_dynamicUpdate = ConfigurationHelper.GetBoolean(config, "dynamicUpdate", false);
			config.Remove("dynamicUpdate");

			_enableAutoUnlock = ConfigurationHelper.GetBoolean(config, "enableAutoUnlock", false);
			config.Remove("enableAutoUnlock");

			_autoUnlockMinutes = ConfigurationHelper.GetInt32(config, "autoUnlockMinutes", 30, 1, Int32.MaxValue);
			config.Remove("autoUnlockMinutes");

			_ignoreCreateUserMethod = ConfigurationHelper.GetBoolean(config, "ignoreCreateUserMethod", false);
			config.Remove("ignoreCreateUserMethod");

			// if there is any parameter not processed, error!
			if (config.Count > 0) {
				string nonProcessedAttributte = config.GetKey(0);
				if (!String.IsNullOrEmpty(nonProcessedAttributte)) {
					throw new ProviderException(String.Format(CultureInfo.CurrentCulture, "Unrecognized attribute {0} found in the provider settings", nonProcessedAttributte));
				}
			}

			// Generate a mapping based on the configuration properties
			Configuration cfig = ConfigureNHibernate().Configure();

			// builds the session factory
			_sessionFactory = cfig.BuildSessionFactory();
		}

		/// <summary>Dispose resources used by the session factory.</summary>
		[SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
		~NHCustomMembershipProvider()
		{
			if (_sessionFactory != null) {
				_sessionFactory.Close();
			}
		}

		#endregion

		#region NHibernate related Methods

		/// <summary>Creates the configuration object with the settings to use NHibernate.</summary>
		/// <returns>The NHibernate Configuration object.</returns>
		protected virtual Configuration ConfigureNHibernate()
		{
			Configuration cfig = new Configuration();

			if (String.IsNullOrEmpty(ConfigurationFile)) {
				cfig.Configure();
			} else {
				string baseDir = AppDomain.CurrentDomain.BaseDirectory;
				string relativeSearchPath = AppDomain.CurrentDomain.RelativeSearchPath;
				string binPath = relativeSearchPath == null ? baseDir : Path.Combine(baseDir, relativeSearchPath);
				cfig.Configure(Path.Combine(binPath, ConfigurationFile));
			}

			// generates the mapping information for the membership entity
			MembershipClassMappingInfo mapInfo = new MembershipClassMappingInfo();
			mapInfo.TableName = _tableName;
			mapInfo.DynamicUpdate = _dynamicUpdate;
			mapInfo.IdColumnName = _userIdColumn;
			mapInfo.IdColumnType = _userIdType;
			mapInfo.GeneratorStrategy = _idGeneratorClass;
			mapInfo.UserNameColumn = _userNameColumn;
			mapInfo.UserNameColumnSize = _userNameColumnSize;
			mapInfo.EmailColumn = _emailColumn;
			mapInfo.EmailColumnSize = _emailColumnSize;
			mapInfo.UniqueEmail = _requiresUniqueEmail;
			mapInfo.PasswordColumn = _passwordColumn;
			mapInfo.PasswordColumnSize = _passwordColumnSize;
			mapInfo.PasswordSaltColumn = _passwordSaltColumn;
			mapInfo.PasswordSaltColumnSize = _passwordSaltColumnSize;
			mapInfo.PasswordFormatColumn = _passwordFormatColumn;
			mapInfo.PasswordQuestionColumn = _passwordQuestionColumn;
			mapInfo.PasswordQuestionColumnSize = _passwordQuestionColumnSize;
			mapInfo.PasswordAnswerColumn = _passwordAnswerColumn;
			mapInfo.PasswordAnswerColumnSize = _passwordAnswerColumnSize;
			mapInfo.FailedPasswordAttemptCountColumn = _failedPasswordAttemptCountColumn;
			mapInfo.FailedPasswordAttemptWindowStartColumn = _failedPasswordAttemptWindowStartColumn;
			mapInfo.FailedPasswordAnswerAttemptCountColumn = _failedPasswordAnswerAttemptCountColumn;
			mapInfo.FailedPasswordAnswerAttemptWindowStartColumn = _failedPasswordAnswerAttemptWindowStartColumn;
			mapInfo.LastPasswordChangedDateColumn = _lastPasswordChangedDateColumn;
			mapInfo.CreationDateColumn = _creationDateColumn;
			mapInfo.LastActivityDateColumn = _lastActivityDateColumn;
			mapInfo.IsApprovedColumn = _isApprovedColumn;
			mapInfo.IsLockedOutColumn = _isLockedOutColumn;
			mapInfo.LastLockOutDateColumn = _lastLockOutDateColumn;
			mapInfo.LastLoginDateColumn = _lastLoginDateColumn;
			mapInfo.CommentsColumn = _commentsColumn;

			// creates the membership mapping
			XmlDocument doc = GenerateClassMapping(mapInfo);
			cfig.AddDocument(doc);

			// sets the interceptor to handle UTF DateTimes and MembershipUserInfo default values properly
			cfig.SetInterceptor(new ProviderInterceptor(PasswordFormat));

			return cfig;
		}

		/// <summary>Generates the class mapping for a MembershipUserInfo entity.</summary>
		/// <param name="mapInfo">Information about the mapping for the membership provider.</param>
		/// <returns>An Xml document with the class mapping.</returns>
		[SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
		[SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "We are not normalizing strings")]
		protected static XmlDocument GenerateClassMapping(MembershipClassMappingInfo mapInfo)
		{
			XmlElement classNode;

			XmlDocument doc = XmlDocHelper.CreateClassMappingDocument("MembershipUserInfo", mapInfo.TableName, out classNode);

			// creates the dynamic update attribute
			classNode.Attributes.Append(XmlDocHelper.CreateAttribute(doc, "dynamic-update", mapInfo.DynamicUpdate.ToString().ToLower(CultureInfo.InvariantCulture)));

			IDictionary<string, string> attributes = new Dictionary<string, string>();

			// creates the id and generator
			attributes.Clear();
			attributes["name"] = "UserId";
			attributes["column"] = mapInfo.IdColumnName;
			attributes["type"] = mapInfo.IdColumnType;
			XmlElement idNode = XmlDocHelper.CreateElementWithAttributes(doc, "id", attributes);

			attributes.Clear();
			attributes["class"] = mapInfo.GeneratorStrategy;
			XmlElement generatorNode = XmlDocHelper.CreateElementWithAttributes(doc, "generator", attributes);
			idNode.AppendChild(generatorNode);
			classNode.AppendChild(idNode);

			// generates the other properties if necessary
			TryAddPropertyMapping(doc, classNode, "UserName", mapInfo.UserNameColumn, "String", true, false, mapInfo.UserNameColumnSize);
			TryAddPropertyMapping(doc, classNode, "Email", mapInfo.EmailColumn, "String", mapInfo.UniqueEmail, false, mapInfo.EmailColumnSize);
			TryAddPropertyMapping(doc, classNode, "Password", mapInfo.PasswordColumn, "String", false, false, mapInfo.PasswordColumnSize);
			TryAddPropertyMapping(doc, classNode, "PasswordSalt", mapInfo.PasswordSaltColumn, "String", false, true, mapInfo.PasswordSaltColumnSize);
			TryAddPropertyMapping(doc, classNode, "PasswordFormat", mapInfo.PasswordFormatColumn, typeof(MembershipPasswordFormat).AssemblyQualifiedName, false, false, -1);
			TryAddPropertyMapping(doc, classNode, "PasswordQuestion", mapInfo.PasswordQuestionColumn, "String", false, false, mapInfo.PasswordQuestionColumnSize);
			TryAddPropertyMapping(doc, classNode, "PasswordAnswer", mapInfo.PasswordAnswerColumn, "String", false, false, mapInfo.PasswordAnswerColumnSize);
			TryAddPropertyMapping(doc, classNode, "FailedPasswordAttemptCount", mapInfo.FailedPasswordAttemptCountColumn, "Int32", false, false, -1);
			TryAddPropertyMapping(doc, classNode, "FailedPasswordAttemptWindowStart", mapInfo.FailedPasswordAttemptWindowStartColumn, "DateTime", false, true, -1);
			TryAddPropertyMapping(doc, classNode, "FailedPasswordAnswerAttemptCount", mapInfo.FailedPasswordAnswerAttemptCountColumn, "Int32", false, false, -1);
			TryAddPropertyMapping(doc, classNode, "FailedPasswordAnswerAttemptWindowStart", mapInfo.FailedPasswordAnswerAttemptWindowStartColumn, "DateTime", false, true, -1);
			TryAddPropertyMapping(doc, classNode, "LastPasswordChangedDate", mapInfo.LastPasswordChangedDateColumn, "DateTime", false, true, -1);
			TryAddPropertyMapping(doc, classNode, "CreationDate", mapInfo.CreationDateColumn, "DateTime", false, false, -1);
			TryAddPropertyMapping(doc, classNode, "LastActivityDate", mapInfo.LastActivityDateColumn, "DateTime", false, false, -1);
			TryAddPropertyMapping(doc, classNode, "IsApproved", mapInfo.IsApprovedColumn, "Boolean", false, false, -1);
			TryAddPropertyMapping(doc, classNode, "IsLockedOut", mapInfo.IsLockedOutColumn, "Boolean", false, false, -1);
			TryAddPropertyMapping(doc, classNode, "LastLockOutDate", mapInfo.LastLockOutDateColumn, "DateTime", false, true, -1);
			TryAddPropertyMapping(doc, classNode, "LastLoginDate", mapInfo.LastLoginDateColumn, "DateTime", false, true, -1);
			TryAddPropertyMapping(doc, classNode, "Comments", mapInfo.CommentsColumn, "String", false, true, -1);

			return doc;
		}

		/// <summary>Creates a mapping between a column and a property if the property will be used.</summary>
		/// <param name="doc">The associated mapping document.</param>
		/// <param name="classNode">The associated class node.</param>
		/// <param name="propertyName">Name of the property to map.</param>
		/// <param name="columnName">Name of the column to map.</param>
		/// <param name="type">The type.</param>
		/// <param name="isUnique">if set to true, mark the column as unique.</param>
		/// <param name="isNullable">if set to true mark the column as nullable.</param>
		/// <param name="length">The maximum length of the column or -1 if it isn't specified.</param>
		[SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
		[SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
		protected static void TryAddPropertyMapping(XmlDocument doc, XmlElement classNode, string propertyName, string columnName, string type, bool isUnique, bool isNullable, int length)
		{
			// if the data has not been supplied, exit
			if (String.IsNullOrEmpty(columnName) || String.IsNullOrEmpty(propertyName)) {
				return;
			}

			IDictionary<string, string> attributes = new Dictionary<string, string>();
			attributes["name"] = propertyName;
			attributes["column"] = columnName;
			attributes["type"] = type;
			attributes["unique"] = isUnique.ToString().ToLowerInvariant();
			attributes["not-null"] = (!isNullable).ToString().ToLowerInvariant();
			if (length != -1) {
				attributes["length"] = length.ToString(CultureInfo.InvariantCulture);
			}
			XmlElement propertyNode = XmlDocHelper.CreateElementWithAttributes(doc, "property", attributes);
			classNode.AppendChild(propertyNode);
		}

		#endregion

		#region Password related Methods

		/// <summary>Processes a request to update the password for a membership user.</summary>
		/// <param name="username">The user to update the password for.</param>
		/// <param name="oldPassword">The current password for the specified user.</param>
		/// <param name="newPassword">The new password for the specified user.</param>
		/// <returns>true if the password was updated successfully; otherwise, false.</returns>
		public override bool ChangePassword(string username, string oldPassword, string newPassword)
		{
			// check if the parameters are valid
			CheckParameter(username, "username", true, true, _userNameColumnSize);
			CheckParameter(newPassword, "newPassword", true, true, _passwordColumnSize);

			// executes the following code inside a transaction
			return Inside.Transaction<bool>(_sessionFactory, delegate(ISession session, ITransaction trans) {
			                                                 	MembershipUserInfo mui;

			                                                 	// check if the password is correct, updating the info about password attempts if necessary
			                                                 	if (!CheckPassword(session, username, oldPassword, true, out mui)) {
			                                                 		return false;
			                                                 	}

			                                                 	// check if the new password passes all required conditions
                                                                //if (!CheckPasswordPolicy(newPassword)) {
                                                                //    return false;
                                                                //}

			                                                 	// encrypt the new password
			                                                 	string encodedPassword = EncodePassword(newPassword, mui.PasswordFormat, mui.PasswordSalt);

			                                                 	if ((_passwordColumnSize != -1) && (encodedPassword.Length > _passwordColumnSize)) {
			                                                 		throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "The encoded password is too long ({0} characters).", encodedPassword.Length));
			                                                 	}

			                                                 	// throws the validating password event
			                                                 	ValidatePasswordEventArgs e;
			                                                 	if (!ValidatePassword(username, newPassword, out e)) {
			                                                 		if (e.FailureInformation != null) {
			                                                 			throw e.FailureInformation;
			                                                 		} else {
			                                                 			throw new ArgumentException("The custom password validation failed.");
			                                                 		}
			                                                 	}

			                                                 	// finally, change the password
			                                                 	mui.Password = encodedPassword;
			                                                 	mui.LastPasswordChangedDate = DateTime.UtcNow;

			                                                 	return true;
			                                                 });
		}

		/// <summary>Gets the password for the specified user name from the data source.</summary>
		/// <param name="username">The user to retrieve the password for.</param>
		/// <param name="answer">The password answer for the user.</param>
		/// <returns>The password for the specified user name.</returns>
		public override string GetPassword(string username, string answer)
		{
			// check if password retrieval is enabled
			if (!EnablePasswordRetrieval) {
				throw new NotSupportedException("Error, the provider has not been configured for password retrieval.");
			}

			// check if the parameters are valid
			CheckParameter(username, "username", true, true, _userNameColumnSize);
			CheckParameter(answer, "answer", RequiresQuestionAndAnswer, RequiresQuestionAndAnswer, _passwordAnswerColumnSize);

			// executes the following code inside a transaction
			return Inside.Transaction<string>(_sessionFactory, delegate(ISession session, ITransaction trans) {
			                                                   	// gets the encoded answer
			                                                   	MembershipUserInfo mui;
			                                                   	string encodedPasswordAnswer = GetEncodedStringByUserSettings(session, username, answer, out mui);

			                                                   	if (mui.IsLockedOut && !EnableAutoUnlock) {
			                                                   		throw new ProviderException("Error, the account has been locked out.");
			                                                   	}

			                                                   	if (RequiresQuestionAndAnswer && String.IsNullOrEmpty(encodedPasswordAnswer)) {
			                                                   		return String.Empty;
			                                                   	}

			                                                   	if (RequiresQuestionAndAnswer) {
			                                                   		// if the password answer is not correct, update attempt data
			                                                   		if (encodedPasswordAnswer != mui.PasswordAnswer) {
			                                                   			DateTime now = DateTime.UtcNow;

			                                                   			// if the current try has been done after the attempt window, init the attempts
			                                                   			if (!mui.FailedPasswordAnswerAttemptWindowStart.HasValue || (mui.FailedPasswordAnswerAttemptWindowStart.Value.AddMinutes(PasswordAttemptWindow) <= now)) {
			                                                   				mui.FailedPasswordAnswerAttemptCount = 1;
			                                                   				mui.FailedPasswordAnswerAttemptWindowStart = now;
			                                                   			} else {
			                                                   				// otherwise, increment the number of attempts
			                                                   				mui.FailedPasswordAnswerAttemptCount++;
			                                                   				mui.FailedPasswordAnswerAttemptWindowStart = now;
			                                                   			}

			                                                   			// if the maximum number of attempts has been exceeded, lock the account
			                                                   			if (mui.FailedPasswordAnswerAttemptCount >= MaxInvalidPasswordAttempts) {
			                                                   				mui.IsLockedOut = true;
			                                                   				mui.LastLockOutDate = now;
			                                                   			}

			                                                   			trans.Commit();

			                                                   			throw new MembershipPasswordException("The password-answer supplied is invalid");
			                                                   		} else {
			                                                   			// if the user is locked and the password answer is correct, try to auto unlock it (if that option is enabled)
			                                                   			if (mui.IsLockedOut && !AutoUnlockUser(mui)) {
			                                                   				throw new ProviderException("Error, the account has been locked out.");
			                                                   			}

			                                                   			// if the password answer is correct and there was any failed attempt, reset attempt data
			                                                   			if (mui.FailedPasswordAnswerAttemptCount > 0) {
			                                                   				mui.FailedPasswordAnswerAttemptCount = 0;
			                                                   				mui.FailedPasswordAnswerAttemptWindowStart = null;
			                                                   			}
			                                                   		}
			                                                   	}

			                                                   	return DecodePassword(mui.Password, mui.PasswordFormat);
			                                                   });
		}

		/// <summary>Resets a user's password to a new, automatically generated password.</summary>
		/// <param name="username">The user to reset the password for.</param>
		/// <param name="answer">The password answer for the specified user.</param>
		/// <returns>The new password for the specified user.</returns>
		public override string ResetPassword(string username, string answer)
		{
			// check if password reset is enabled
			if (!EnablePasswordReset) {
				throw new NotSupportedException("Error, the provider has not been configured for password reset.");
			}

			// check if the parameters are valid
			CheckParameter(username, "username", true, true, _userNameColumnSize);
			CheckParameter(answer, "answer", RequiresQuestionAndAnswer, RequiresQuestionAndAnswer, _passwordAnswerColumnSize);

			// executes the following code inside a transaction
			return Inside.Transaction<string>(_sessionFactory, delegate(ISession session, ITransaction trans) {
			                                                   	// gets the encoded answer
			                                                   	MembershipUserInfo mui;
			                                                   	string encodedPasswordAnswer = GetEncodedStringByUserSettings(session, username, answer, out mui);

			                                                   	if (mui.IsLockedOut && !EnableAutoUnlock) {
			                                                   		throw new ProviderException("Error, the account has been locked out.");
			                                                   	}

			                                                   	if (RequiresQuestionAndAnswer && String.IsNullOrEmpty(encodedPasswordAnswer)) {
			                                                   		return String.Empty;
			                                                   	}

			                                                   	if (RequiresQuestionAndAnswer) {
			                                                   		// if the password answer is not correct, update attempt data
			                                                   		if (encodedPasswordAnswer != mui.PasswordAnswer) {
			                                                   			DateTime now = DateTime.UtcNow;

			                                                   			// if the current try has been done after the attempt window, init the attempts
			                                                   			if (!mui.FailedPasswordAnswerAttemptWindowStart.HasValue || (mui.FailedPasswordAnswerAttemptWindowStart.Value.AddMinutes(PasswordAttemptWindow) <= now)) {
			                                                   				mui.FailedPasswordAnswerAttemptCount = 1;
			                                                   				mui.FailedPasswordAnswerAttemptWindowStart = now;
			                                                   			} else {
			                                                   				// otherwise, increment the number of attempts
			                                                   				mui.FailedPasswordAnswerAttemptCount++;
			                                                   				mui.FailedPasswordAnswerAttemptWindowStart = now;
			                                                   			}

			                                                   			// if the maximum number of attempts has been exceeded, lock the account
			                                                   			if (mui.FailedPasswordAnswerAttemptCount >= MaxInvalidPasswordAttempts) {
			                                                   				mui.IsLockedOut = true;
			                                                   				mui.LastLockOutDate = now;
			                                                   			}

			                                                   			trans.Commit();

			                                                   			throw new MembershipPasswordException("The password-answer supplied is invalid");
			                                                   		} else {
			                                                   			// if the user is locked and the password answer is correct, try to auto unlock it (if that option is enabled)
			                                                   			if (mui.IsLockedOut && !AutoUnlockUser(mui)) {
			                                                   				throw new ProviderException("Error, the account has been locked out.");
			                                                   			}

			                                                   			// if the password answer is correct and there was any failed attempt, reset attempt data
			                                                   			if (mui.FailedPasswordAnswerAttemptCount > 0) {
			                                                   				mui.FailedPasswordAnswerAttemptCount = 0;
			                                                   				mui.FailedPasswordAnswerAttemptWindowStart = null;
			                                                   			}
			                                                   		}
			                                                   	} else {
			                                                   		// if the user is locked, try to auto unlock it (if that option is enabled)
			                                                   		if (mui.IsLockedOut && !AutoUnlockUser(mui)) {
			                                                   			throw new ProviderException("Error, the account has been locked out.");
			                                                   		}
			                                                   	}

			                                                   	// generates a new password
			                                                   	string newPassword = Membership.GeneratePassword((MinRequiredPasswordLength < PASSWORD_SIZE) ? PASSWORD_SIZE : MinRequiredPasswordLength, MinRequiredNonAlphanumericCharacters);

			                                                   	// throws the validating password event
			                                                   	ValidatePasswordEventArgs e;
			                                                   	if (!ValidatePassword(username, newPassword, out e)) {
			                                                   		if (e.FailureInformation != null) {
			                                                   			throw e.FailureInformation;
			                                                   		} else {
			                                                   			throw new ProviderException("The custom password validation failed.");
			                                                   		}
			                                                   	}

			                                                   	// encrypt the new password
			                                                   	string encodedPassword = EncodePassword(newPassword, mui.PasswordFormat, mui.PasswordSalt);

			                                                   	if ((_passwordColumnSize != -1) && (encodedPassword.Length > _passwordColumnSize)) {
			                                                   		throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "The encoded password is too long ({0} characters).", encodedPassword.Length));
			                                                   	}

			                                                   	// finally, change the password
			                                                   	mui.Password = encodedPassword;
			                                                   	mui.LastPasswordChangedDate = DateTime.UtcNow;

			                                                   	return newPassword;
			                                                   });
		}

		/// <summary>Processes a request to update the password question and answer for a membership user.</summary>
		/// <param name="username">The user to change the password question and answer for.</param>
		/// <param name="password">The password for the specified user.</param>
		/// <param name="newPasswordQuestion">The new password question for the specified user.</param>
		/// <param name="newPasswordAnswer">The new password answer for the specified user.</param>
		/// <returns>true if the password question and answer are updated successfully; otherwise, false.</returns>
		public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
		{
			// check if the parameters are valid
			CheckParameter(username, "username", true, true, _userNameColumnSize);
			CheckParameter(password, "password", true, true, _passwordColumnSize);
			CheckParameter(newPasswordQuestion, "newPasswordQuestion", RequiresQuestionAndAnswer, RequiresQuestionAndAnswer, _passwordQuestionColumnSize);
			CheckParameter(newPasswordAnswer, "newPasswordAnswer", RequiresQuestionAndAnswer, RequiresQuestionAndAnswer, _passwordAnswerColumnSize);

			// executes the following code inside a transaction
			return Inside.Transaction<bool>(_sessionFactory, delegate(ISession session, ITransaction trans) {
			                                                 	MembershipUserInfo mui;

			                                                 	// check if the password is correct, updating the info about password attempts if necessary
			                                                 	if (!CheckPassword(session, username, password, true, out mui)) {
			                                                 		return false;
			                                                 	}

			                                                 	// encodes the new password answer
			                                                 	string encodedAnswer = newPasswordAnswer;

			                                                 	if (!String.IsNullOrEmpty(newPasswordAnswer)) {
			                                                 		encodedAnswer = EncodePassword(newPasswordAnswer, mui.PasswordFormat, mui.PasswordSalt);

			                                                 		if ((_passwordAnswerColumnSize != -1) && (encodedAnswer.Length > _passwordAnswerColumnSize)) {
			                                                 			throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "The encoded answer is too long. It must not exceed the {0} characters.", _passwordAnswerColumnSize));
			                                                 		}
			                                                 	}

			                                                 	// finally, change the question and answer
			                                                 	mui.PasswordQuestion = newPasswordQuestion;
			                                                 	mui.PasswordAnswer = encodedAnswer;

			                                                 	return true;
			                                                 });
		}

		/// <summary>Clears a lock so that the membership user can be validated.</summary>
		/// <param name="userName">The membership user to clear the lock status for.</param>
		/// <returns>true if the membership user was successfully unlocked; otherwise, false.</returns>
		public override bool UnlockUser(string userName)
		{
			// check if the parameters are valid
			CheckParameter(userName, "userName", true, true, _userNameColumnSize);

			// executes the following code inside a transaction
			return Inside.Transaction<bool>(_sessionFactory, delegate(ISession session, ITransaction trans) {
			                                                 	// gets the membership data for this user
			                                                 	MembershipUserInfoDal muiDAL = new MembershipUserInfoDal(session);
			                                                 	MembershipUserInfo mui = muiDAL.GetByUserName(userName);

			                                                 	if (mui == null) {
			                                                 		return false;
			                                                 	}

			                                                 	// unlock user and reset attempt data
			                                                 	mui.IsLockedOut = false;
			                                                 	mui.FailedPasswordAttemptCount = 0;
			                                                 	mui.FailedPasswordAttemptWindowStart = null;
			                                                 	mui.FailedPasswordAnswerAttemptCount = 0;
			                                                 	mui.FailedPasswordAnswerAttemptWindowStart = null;
			                                                 	mui.LastLockOutDate = null;

			                                                 	return true;
			                                                 });
		}

		/// <summary>Verifies that the specified user name and password exist in the data source.</summary>
		/// <param name="username">The name of the user to validate.</param>
		/// <param name="password">The password for the specified user.</param>
		/// <returns>true if the specified username and password are valid; otherwise, false.</returns>
		public override bool ValidateUser(string username, string password)
		{
			// check if the parameters are valid
			CheckParameter(username, "username", true, true, _userNameColumnSize);
			CheckParameter(password, "password", true, true, _passwordColumnSize);

			// executes the following code inside a transaction
			return Inside.Transaction<bool>(_sessionFactory, delegate(ISession session, ITransaction trans) {
			                                                 	MembershipUserInfo mui;

			                                                 	// check if the password is correct, updating the info about password attempts if necessary
			                                                 	bool validPassword = CheckPassword(session, username, password, true, out mui);

			                                                 	// raise system events
			                                                 	if (RaiseSystemEvents) {
			                                                 		if (validPassword) {
			                                                 			WebEventsHelper.RaiseSystemEvent(null, this, WebEventCodes.AuditFileAuthorizationSuccess, 0, null, username);
			                                                 		} else {
			                                                 			WebEventsHelper.RaiseSystemEvent(null, this, WebEventCodes.AuditMembershipAuthenticationFailure, 0, null, username);
			                                                 		}
			                                                 		// NOTE: the original SqlMembershipProvider also increments the performance counters AppPerfCounter.MEMBER_SUCCESS and AppPerfCounter.MEMBER_FAIL
			                                                 	}

			                                                 	return validPassword;
			                                                 });
		}

		#endregion

		#region Create/Update/Delete MembershipUser

		/// <summary>Adds a new membership user to the data source.</summary>
		/// <remarks>As this provider can work on an existing users table, the application must create the users
		/// directly if the data in the users table that is not part of the membership data can't be null.
		/// To create users directly, use the ValidateUserCreation method to get the required data 
		/// and perform the necessary checks.</remarks>
		/// <param name="username">The user name for the new user.</param>
		/// <param name="password">The password for the new user.</param>
		/// <param name="email">The e-mail address for the new user.</param>
		/// <param name="passwordQuestion">The password question for the new user.</param>
		/// <param name="passwordAnswer">The password answer for the new user</param>
		/// <param name="isApproved">Whether or not the new user is approved to be validated.</param>
		/// <param name="providerUserKey">The unique identifier from the membership data source for the user.</param>
		/// <param name="status">A MembershipCreateStatus enumeration value indicating whether the user was created successfully.</param>
		/// <returns>A MembershipUser object populated with the information for the newly created user.</returns>
		public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
		{
			// check if the parameters are valid
			if (!ValidateParameter(username, true, true, _userNameColumnSize)) {
				status = MembershipCreateStatus.InvalidUserName;
				return null;
			}

			if (!ValidateParameter(password, true, true, _passwordColumnSize)) {
				status = MembershipCreateStatus.InvalidPassword;
				return null;
			}

			if (!ValidateParameter(email, true, true, _emailColumnSize)) {
				status = MembershipCreateStatus.InvalidEmail;
				return null;
			}

			if (!ValidateParameter(passwordQuestion, RequiresQuestionAndAnswer, RequiresQuestionAndAnswer, _passwordQuestionColumnSize)) {
				status = MembershipCreateStatus.InvalidQuestion;
				return null;
			}
			if (!ValidateParameter(passwordAnswer, RequiresQuestionAndAnswer, RequiresQuestionAndAnswer, _passwordAnswerColumnSize)) {
				status = MembershipCreateStatus.InvalidAnswer;
				return null;
			}

			// check if the new password passes all required conditions
            //if (!CheckPasswordPolicy(password)) {
            //    status = MembershipCreateStatus.InvalidPassword;
            //    return null;
            //}

			// encrypt the new password
			string passwordSalt = (_passwordSaltColumn != null) ? GeneratePasswordSalt(PasswordFormat, PasswordSaltSize) : null;

			if ((_passwordSaltColumnSize != -1) && (passwordSalt != null) && (passwordSalt.Length > _passwordSaltColumnSize)) {
				status = MembershipCreateStatus.InvalidPassword;
				return null;
			}

			string encodedPassword = EncodePassword(password, PasswordFormat, passwordSalt);

			if ((_passwordColumnSize != -1) && (encodedPassword.Length > _passwordColumnSize)) {
				status = MembershipCreateStatus.InvalidPassword;
				return null;
			}

			// throws the validating password event
			ValidatePasswordEventArgs e;
			if (!ValidatePassword(username, password, out e)) {
				status = MembershipCreateStatus.InvalidPassword;
				return null;
			}

			string encodedAnswer = passwordAnswer;

			// encodes the password answer
			if (!String.IsNullOrEmpty(passwordAnswer)) {
				encodedAnswer = EncodePassword(passwordAnswer, PasswordFormat, passwordSalt);

				if ((_passwordAnswerColumnSize != -1) && (encodedAnswer.Length > _passwordAnswerColumnSize)) {
					status = MembershipCreateStatus.InvalidAnswer;
					return null;
				}
			}

			object[] result = Inside.Transaction<object[]>(_sessionFactory, delegate(ISession session, ITransaction trans) {
			                                                                	MembershipUserInfoDal muiDAL = new MembershipUserInfoDal(session);

			                                                                	// try to get an user with that user name
			                                                                	MembershipUserInfo mui = muiDAL.GetByUserName(username);

			                                                                	if (mui != null) {
			                                                                		return new object[] { MembershipCreateStatus.DuplicateUserName, null };
			                                                                	}

			                                                                	// try to get an user with that user id
			                                                                	if (providerUserKey != null) {
			                                                                		mui = muiDAL.GetById(providerUserKey);

			                                                                		if (mui != null) {
			                                                                			return new object[] { MembershipCreateStatus.DuplicateProviderUserKey, null };
			                                                                		}
			                                                                	}

			                                                                	if (RequiresUniqueEmail) {
			                                                                		// get the users names with that email
			                                                                		IList users = muiDAL.GetUserNameByEmail(email);

			                                                                		if (users.Count > 0) {
			                                                                			return new object[] { MembershipCreateStatus.DuplicateEmail, null };
			                                                                		}
			                                                                	}

			                                                                	DateTime now = DateTime.UtcNow;

			                                                                	// finally, save the user
			                                                                	mui = new MembershipUserInfo();
			                                                                	mui.UserId = providerUserKey;
			                                                                	mui.UserName = username;
			                                                                	mui.Email = email;
			                                                                	mui.Password = encodedPassword;
			                                                                	mui.PasswordSalt = passwordSalt;
			                                                                	mui.PasswordFormat = PasswordFormat;
			                                                                	mui.PasswordQuestion = passwordQuestion ?? "";
			                                                                	mui.PasswordAnswer = encodedAnswer ?? "";
			                                                                	mui.LastPasswordChangedDate = now;
			                                                                	mui.CreationDate = now;
			                                                                	mui.LastActivityDate = now;
			                                                                	mui.IsApproved = isApproved;
			                                                                	mui.IsLockedOut = false;

			                                                                	// if we do not want to create an user with the basic data, skip the insert
			                                                                	if (!IgnoreCreateUserMethod) {
			                                                                		session.Save(mui);
			                                                                	}

			                                                                	return new object[] { MembershipCreateStatus.Success, mui.CreateMembershipUser(Name) };
			                                                                });

			status = (MembershipCreateStatus)result[0];

			return (MembershipUser)result[1];
		}

		/// <summary>Validates the creation of a new user, generation also the encoded password and answer for the user.</summary>
		/// <remarks>This method is usefull for manually creating your own users with custom data. This method
		/// is called internally by the CreateUserExWizard to allow creating the user in the CreateUserEx event.</remarks>
		/// <param name="userName">The user name for the new user.</param>
		/// <param name="password">The password for the new user.</param>
		/// <param name="email">The e-mail address for the new user.</param>
		/// <param name="passwordQuestion">The password question for the new user.</param>
		/// <param name="passwordAnswer">The password answer for the new user</param>
		/// <param name="isApproved">Whether or not the new user is approved to be validated.</param>
		/// <param name="providerUserKey">The unique identifier from the membership data source for the user.</param>
		/// <param name="status">A MembershipCreateStatus enumeration value indicating whether the user can be created successfully.</param>
		/// <param name="generatedPassword">The generated password for the user that will be created.</param>
		/// <param name="generatedPasswordSalt">The generated password salt for the user that will be created.</param>
		/// <param name="generatedAnswer">The generated answer for the user that will be created.</param>
		/// <returns>A MembershipUser object populated with the information for the newly created user.</returns>
		public virtual bool ValidateUserCreation(string userName, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status, out string generatedPassword, out string generatedPasswordSalt, out string generatedAnswer)
		{
			generatedPassword = null;
			generatedPasswordSalt = null;
			generatedAnswer = passwordAnswer;

			// check if the parameters are valid
			if (!ValidateParameter(userName, true, true, _userNameColumnSize)) {
				status = MembershipCreateStatus.InvalidUserName;
				return false;
			}

			if (!ValidateParameter(password, true, true, _passwordColumnSize)) {
				status = MembershipCreateStatus.InvalidPassword;
				return false;
			}

			if (!ValidateParameter(email, true, true, _emailColumnSize)) {
				status = MembershipCreateStatus.InvalidEmail;
				return false;
			}

			if (!ValidateParameter(passwordQuestion, RequiresQuestionAndAnswer, RequiresQuestionAndAnswer, _passwordQuestionColumnSize)) {
				status = MembershipCreateStatus.InvalidQuestion;
				return false;
			}

			if (!ValidateParameter(passwordAnswer, RequiresQuestionAndAnswer, RequiresQuestionAndAnswer, _passwordAnswerColumnSize)) {
				status = MembershipCreateStatus.InvalidAnswer;
				return false;
			}

			// check if the new password passes all required conditions
			if (!CheckPasswordPolicy(password)) {
				status = MembershipCreateStatus.InvalidPassword;
				return false;
			}

			// encrypt the new password
			generatedPasswordSalt = (_passwordSaltColumn != null) ? GeneratePasswordSalt(PasswordFormat, PasswordSaltSize) : null;

			if ((_passwordSaltColumnSize != -1) && (generatedPasswordSalt != null) && (generatedPasswordSalt.Length > _passwordSaltColumnSize)) {
				status = MembershipCreateStatus.InvalidPassword;
				return false;
			}

			generatedPassword = EncodePassword(password, PasswordFormat, generatedPasswordSalt);

			if ((_passwordColumnSize != -1) && (generatedPassword.Length > _passwordColumnSize)) {
				status = MembershipCreateStatus.InvalidPassword;
				return false;
			}

			// throws the validating password event
			ValidatePasswordEventArgs e;
			if (!ValidatePassword(userName, password, out e)) {
				status = MembershipCreateStatus.InvalidPassword;
				return false;
			}

			// encodes the password answer
			if (!String.IsNullOrEmpty(passwordAnswer)) {
				generatedAnswer = EncodePassword(passwordAnswer, PasswordFormat, generatedPasswordSalt);

				if ((_passwordAnswerColumnSize != -1) && (generatedAnswer.Length > _passwordAnswerColumnSize)) {
					status = MembershipCreateStatus.InvalidAnswer;
					return false;
				}
			}

			// executes the following code inside a transaction
			object[] result = Inside.Transaction<object[]>(_sessionFactory, delegate(ISession session, ITransaction trans) {
			                                                                	MembershipUserInfoDal muiDAL = new MembershipUserInfoDal(session);

			                                                                	// try to get an user with that user name
			                                                                	MembershipUserInfo mui = muiDAL.GetByUserName(userName);

			                                                                	if (mui != null) {
			                                                                		return new object[] { MembershipCreateStatus.DuplicateUserName, false };
			                                                                	}

			                                                                	// try to get an user with that user id
			                                                                	if (providerUserKey != null) {
			                                                                		mui = muiDAL.GetById(providerUserKey);

			                                                                		if (mui != null) {
			                                                                			return new object[] { MembershipCreateStatus.DuplicateProviderUserKey, false };
			                                                                		}
			                                                                	}

			                                                                	if (RequiresUniqueEmail) {
			                                                                		// get the users names with that email
			                                                                		IList users = muiDAL.GetUserNameByEmail(email);

			                                                                		if (users.Count > 0) {
			                                                                			return new object[] { MembershipCreateStatus.DuplicateEmail, false };
			                                                                		}
			                                                                	}

			                                                                	// the user can be successfully created
			                                                                	return new object[] { MembershipCreateStatus.Success, true };
			                                                                });

			status = (MembershipCreateStatus)result[0];

			return (bool)result[1];
		}


		/// <summary>Updates information about a user in the data source.</summary>
		/// <param name="user">A MembershipUser object that represents the user to update and the updated information for the user.</param>
		public override void UpdateUser(MembershipUser user)
		{
			if (user.ProviderUserKey == null) {
				throw new ProviderException("Error, the ProviderUserKey must be provided");
			}

			// executes the following code inside a transaction
			Inside.Transaction(_sessionFactory, delegate(ISession session, ITransaction trans) {
			                                    	// gets the membership data for this user
			                                    	MembershipUserInfoDal muiDAL = new MembershipUserInfoDal(session);
			                                    	MembershipUserInfo mui = muiDAL.GetById(user.ProviderUserKey);

			                                    	if (mui == null) {
			                                    		throw new ProviderException("Error, the specified user does not exist");
			                                    	}

			                                    	// modify the user information with the updated information
			                                    	mui.ModifyFromMembershipUser(user);
			                                    });
		}

		/// <summary>Removes a user from the membership data source.</summary>
		/// <param name="username">The name of the user to delete.</param>
		/// <param name="deleteAllRelatedData">true to delete data related to the user from the database; false to leave data related to the user in the database.</param>
		/// <returns>true if the user was successfully deleted; otherwise, false.</returns>
		public override bool DeleteUser(string username, bool deleteAllRelatedData)
		{
			// TODO: for now, ignore deleteAllRelatedData and use database ON CASCADE option.

			// check if the parameters are valid
			CheckParameter(username, "username", true, true, _userNameColumnSize);

			// executes the following code inside a transaction
			return Inside.Transaction<bool>(_sessionFactory, delegate(ISession session, ITransaction trans) {
			                                                 	// gets the membership data for this user
			                                                 	MembershipUserInfoDal muiDAL = new MembershipUserInfoDal(session);
			                                                 	MembershipUserInfo mui = muiDAL.GetByUserName(username);

			                                                 	if (mui == null) {
			                                                 		return false;
			                                                 	}

			                                                 	return muiDAL.Delete(mui);
			                                                 });
		}

		#endregion

		#region Queries

		/// <summary>Gets information from the data source for a user based on the unique identifier for the membership user. Provides an option to update the last-activity date/time stamp for the user.</summary>
		/// <param name="providerUserKey">The unique identifier for the membership user to get information for.</param>
		/// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
		/// <returns>A MembershipUser object populated with the specified user's information from the data source or null if the user was not found.</returns>
		public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
		{
			if (providerUserKey == null) {
				throw new ProviderException("Error, the ProviderUserKey must be provided");
			}

			// executes the following code inside a transaction
			return Inside.Transaction<MembershipUser>(_sessionFactory, delegate(ISession session, ITransaction trans) {
			                                                           	// gets the membership data for this user
			                                                           	MembershipUserInfoDal muiDAL = new MembershipUserInfoDal(session);
			                                                           	MembershipUserInfo mui = muiDAL.GetById(providerUserKey);

			                                                           	if (mui == null) {
			                                                           		return null;
			                                                           	}

			                                                           	if (userIsOnline) {
			                                                           		mui.LastActivityDate = DateTime.UtcNow;
			                                                           	}

			                                                           	return mui.CreateMembershipUser(Name);
			                                                           });
		}

		/// <summary>Gets information from the data source for a user. Provides an option to update the last-activity date/time stamp for the user.</summary>
		/// <param name="username">The name of the user to get information for.</param>
		/// <param name="userIsOnline">true to update the last-activity date/time stamp for the user; false to return user information without updating the last-activity date/time stamp for the user.</param>
		/// <returns>A MembershipUser object populated with the specified user's information from the data source.</returns>
		public override MembershipUser GetUser(string username, bool userIsOnline)
		{
			// check if the parameters are valid
			CheckParameter(username, "username", true, true, _userNameColumnSize);

			// executes the following code inside a transaction
			return Inside.Transaction<MembershipUser>(_sessionFactory, delegate(ISession session, ITransaction trans) {
			                                                           	// gets the membership data for this user
			                                                           	MembershipUserInfoDal muiDAL = new MembershipUserInfoDal(session);
			                                                           	MembershipUserInfo mui = muiDAL.GetByUserName(username);

			                                                           	if (mui == null) {
			                                                           		return null;
			                                                           	}

			                                                           	if (userIsOnline) {
			                                                           		mui.LastActivityDate = DateTime.UtcNow;
			                                                           	}

			                                                           	return mui.CreateMembershipUser(Name);
			                                                           });
		}

		/// <summary>Gets the user name associated with the specified e-mail address.</summary>
		/// <param name="email">The e-mail address to search for.</param>
		/// <returns>The user name associated with the specified e-mail address. If no match is found, return null.</returns>
		public override string GetUserNameByEmail(string email)
		{
			// check if the parameters are valid
			CheckParameter(email, "email", true, true, _emailColumnSize);

			// executes the following code inside a transaction
			return Inside.Transaction<string>(_sessionFactory, delegate(ISession session, ITransaction trans) {
			                                                   	// gets the membership data for this user
			                                                   	MembershipUserInfoDal muiDAL = new MembershipUserInfoDal(session);
			                                                   	IList users = muiDAL.GetUserNameByEmail(email);

			                                                   	if ((users == null) || (users.Count == 0)) {
			                                                   		return null;
			                                                   	}

			                                                   	if (RequiresUniqueEmail && (users.Count > 1)) {
			                                                   		throw new ProviderException("More than one user has the specified e-mail address");
			                                                   	}

			                                                   	return users[0] as string;
			                                                   });
		}

		/// <summary>Gets a collection of membership users where the e-mail address contains the specified e-mail address to match.</summary>
		/// <param name="emailToMatch">The e-mail address to search for.</param>
		/// <param name="pageIndex">The index of the page of results to return. pageIndex is zero-based.</param>
		/// <param name="pageSize">The size of the page of results to return.</param>
		/// <param name="totalRecords">The total number of matched users.</param>
		/// <returns>A MembershipUserCollection collection that contains a page of pageSize MembershipUser objects beginning at the page specified by pageIndex.</returns>
		public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			// check if the parameters are valid
			CheckParameter(emailToMatch, "emailToMatch", true, true, _emailColumnSize);

			if (pageIndex < 0) {
				throw new ArgumentException("Error, the page index can't be negative.");
			}

			if (pageSize < 1) {
				throw new ArgumentException("Error, the page size must be positive.");
			}

			// executes the following code inside a transaction
			object[] result = Inside.Transaction<object[]>(_sessionFactory, delegate(ISession session, ITransaction trans) {
			                                                                	// gets the list of users that meet the search criteria
			                                                                	int numRecords;
			                                                                	MembershipUserInfoDal muiDAL = new MembershipUserInfoDal(session);
			                                                                	IList<MembershipUserInfo> users = muiDAL.GetByEmail(emailToMatch, pageIndex*pageSize, pageSize, out numRecords);

			                                                                	// convert the collection of MembershipUserInfo instances into a MembershipUserCollection
			                                                                	MembershipUserCollection col = new MembershipUserCollection();

			                                                                	foreach (MembershipUserInfo user in users) {
			                                                                		col.Add(user.CreateMembershipUser(Name));
			                                                                	}

			                                                                	return new object[] { col, numRecords };
			                                                                });

			totalRecords = (int)result[1];

			return (MembershipUserCollection)result[0];
		}

		/// <summary>Gets a collection of membership users where the user name contains the specified user name to match.</summary>
		/// <param name="usernameToMatch">The user name to search for.</param>
		/// <param name="pageIndex">The index of the page of results to return. pageIndex is zero-based.</param>
		/// <param name="pageSize">The size of the page of results to return.</param>
		/// <param name="totalRecords">The total number of matched users.</param>
		/// <returns>A MembershipUserCollection collection that contains a page of pageSize MembershipUser objects beginning at the page specified by pageIndex.</returns>
		public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			// check if the parameters are valid
			CheckParameter(usernameToMatch, "usernameToMatch", true, true, _userNameColumnSize);

			if (pageIndex < 0) {
				throw new ArgumentException("Error, the page index can't be negative.");
			}

			if (pageSize < 1) {
				throw new ArgumentException("Error, the page size must be positive.");
			}

			// executes the following code inside a transaction
			object[] result = Inside.Transaction<object[]>(_sessionFactory, delegate(ISession session, ITransaction trans) {
			                                                                	// gets the list of users that meet the search criteria
			                                                                	int numRecords;
			                                                                	MembershipUserInfoDal muiDAL = new MembershipUserInfoDal(session);
			                                                                	IList<MembershipUserInfo> users = muiDAL.GetByUserName(usernameToMatch, pageIndex*pageSize, pageSize, out numRecords);

			                                                                	// convert the collection of MembershipUserInfo instances into a MembershipUserCollection
			                                                                	MembershipUserCollection col = new MembershipUserCollection();

			                                                                	foreach (MembershipUserInfo user in users) {
			                                                                		col.Add(user.CreateMembershipUser(Name));
			                                                                	}

			                                                                	return new object[] { col, numRecords };
			                                                                });

			totalRecords = (int)result[1];

			return (MembershipUserCollection)result[0];
		}

		/// <summary>Gets a collection of all the users in the data source in pages of data.</summary>
		/// <param name="pageIndex">The index of the page of results to return. pageIndex is zero-based.</param>
		/// <param name="pageSize">The size of the page of results to return.</param>
		/// <param name="totalRecords">The total number of matched users.</param>
		/// <returns>A MembershipUserCollection collection that contains a page of pageSize MembershipUser objects beginning at the page specified by pageIndex.</returns>
		public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
		{
			if (pageIndex < 0) {
				throw new ArgumentException("Error, the page index can't be negative.");
			}

			if (pageSize < 1) {
				throw new ArgumentException("Error, the page size must be positive.");
			}

			// executes the following code inside a transaction
			object[] result = Inside.Transaction<object[]>(_sessionFactory, delegate(ISession session, ITransaction trans) {
			                                                                	// gets the list of users
			                                                                	int numRecords;
			                                                                	MembershipUserInfoDal muiDAL = new MembershipUserInfoDal(session);
			                                                                	IList<MembershipUserInfo> users = muiDAL.GetAll(pageIndex*pageSize, pageSize, out numRecords);

			                                                                	// convert the collection of MembershipUserInfo instances into a MembershipUserCollection
			                                                                	MembershipUserCollection col = new MembershipUserCollection();

			                                                                	foreach (MembershipUserInfo user in users) {
			                                                                		col.Add(user.CreateMembershipUser(Name));
			                                                                	}

			                                                                	return new object[] { col, numRecords };
			                                                                });

			totalRecords = (int)result[1];

			return (MembershipUserCollection)result[0];
		}

		/// <summary>Gets the number of users currently accessing the application.</summary>
		/// <returns>The number of users currently accessing the application.</returns>
		public override int GetNumberOfUsersOnline()
		{
			// executes the following code inside a transaction
			return Inside.Transaction<int>(_sessionFactory, delegate(ISession session, ITransaction trans) {
			                                                	// if the LastActivityDate or LastLoginDate are persisted, use them to get the number of users online
			                                                	if (HasLastActivityDate || HasLastLoginDate) {
			                                                		// gets the number of users online
			                                                		MembershipUserInfoDal muiDAL = new MembershipUserInfoDal(session);
			                                                		return muiDAL.GetNumberOfUsersOnline(DateTime.UtcNow.AddMinutes(-Membership.UserIsOnlineTimeWindow), HasLastActivityDate);
			                                                	} else {
			                                                		// otherwise, we can't retrieve the number of users online
			                                                		return -1;
			                                                	}
			                                                });
		}

		#endregion

		#region Password Encoding/Decoding

		/// <summary>Encodes the password based on the password format.</summary>
		/// <param name="password">The password to encode.</param>
		/// <param name="format">The password format to use.</param>
		/// <param name="salt">The salt to use to complement the password.</param>
		/// <returns>The encoded password.</returns>
		public virtual string EncodePassword(string password, MembershipPasswordFormat format, string salt)
		{
			// the password in clear format isn't encoded, so we're done
			if (format == MembershipPasswordFormat.Clear) {
				return password;
			}

			// convert the password and salt to bytes
			byte[] passwordBytes = Encoding.Unicode.GetBytes(password);
			byte[] saltBytes = new byte[0];

			if (salt != null) {
				saltBytes = Convert.FromBase64String(salt);
			}

			// copies the salt and the password to a byte array
			byte[] passWithSaltBytes = new byte[passwordBytes.Length + saltBytes.Length];
			byte[] encodedPassword;

			Buffer.BlockCopy(saltBytes, 0, passWithSaltBytes, 0, saltBytes.Length);
			Buffer.BlockCopy(passwordBytes, 0, passWithSaltBytes, saltBytes.Length, passwordBytes.Length);

			// generates a hash or encrypts the password
			if (format == MembershipPasswordFormat.Hashed) {
				HashAlgorithm hashAlgo = HashAlgorithm.Create(Membership.HashAlgorithmType);
				encodedPassword = hashAlgo.ComputeHash(passWithSaltBytes);
			} else {
				encodedPassword = EncryptPassword(passWithSaltBytes);
			}

			return Convert.ToBase64String(encodedPassword);
		}

		/// <summary>Generates the salt.</summary>
		/// <param name="passwordFormat">The password format.</param>
		/// <param name="passwordSaltSize">Number of bytes to use to generate the password salt.</param>
		/// <returns>A string with the salt.</returns>
		public virtual string GeneratePasswordSalt(MembershipPasswordFormat passwordFormat, int passwordSaltSize)
		{
			// no salt is used if passwords are stored in clear text
			if (passwordFormat == MembershipPasswordFormat.Clear) {
				return null;
			}

			byte[] buf = new byte[passwordSaltSize];
			RNGCryptoServiceProvider cryptoProvider = new RNGCryptoServiceProvider();
			cryptoProvider.GetBytes(buf);
			return Convert.ToBase64String(buf);
		}

		/// <summary>Decodes the password.</summary>
		/// <param name="password">The password to decode.</param>
		/// <param name="format">The password format used to encode it.</param>
		/// <returns>The decoded password.</returns>
		public virtual string DecodePassword(string password, MembershipPasswordFormat format)
		{
			// the password in clear format isn't encoded, so we're done
			if (format == MembershipPasswordFormat.Clear) {
				return password;
			}

			// a hashed password can't be retrieved
			if (format == MembershipPasswordFormat.Hashed) {
				throw new ProviderException("Error, a hased password can't be retrieved");
			}

			// otherwise decrypt the password
			byte[] encodedPassword = Convert.FromBase64String(password);
			byte[] decryptedPassword = DecryptPassword(encodedPassword);
			if (decryptedPassword == null) {
				return null;
			}

			// returns the decoded password ignoring the salt
			int saltLength = (PasswordSaltSize == -1) ? 0 : PasswordSaltSize;
			return Encoding.Unicode.GetString(decryptedPassword, saltLength, decryptedPassword.Length - saltLength);
		}

		#endregion

		#region Helper Methods

		/// <summary>Validates a parameter.</summary>
		/// <param name="param">The parameter to check.</param>
		/// <param name="checkNull">if set to true, the parameter can't be null.</param>
		/// <param name="checkEmpty">if set to true, the parameter can't be empty.</param>
		/// <param name="maxLength">Maximum length of the parameter.</param>
		/// <returns>true if the parameter pass the check; false otherwise.</returns>
		public static bool ValidateParameter(string param, bool checkNull, bool checkEmpty, int maxLength)
		{
			if (param == null) {
				if (checkNull) {
					return false;
				}

				return true;
			}

			if (String.IsNullOrEmpty(param.Trim()) && checkEmpty) {
				return false;
			}

			if ((maxLength != -1) && (param.Length > maxLength)) {
				return false;
			}

			return true;
		}

		/// <summary>Checks if a parameter is correct. If it is not, an exception is thrown.</summary>
		/// <param name="param">The parameter to check.</param>
		/// <param name="paramName">The parameter name.</param>
		/// <param name="checkNull">if set to true, the parameter can't be null.</param>
		/// <param name="checkEmpty">if set to true, the parameter can't be empty.</param>
		/// <param name="maxLength">Maximum length of the parameter.</param>
		public static void CheckParameter(string param, string paramName, bool checkNull, bool checkEmpty, int maxLength)
		{
			if (param == null) {
				if (checkNull) {
					throw new ArgumentNullException(paramName);
				}

				return;
			}

			if (String.IsNullOrEmpty(param.Trim()) && checkEmpty) {
				throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Error, the parameter {0} can not be empty", paramName));
			}

			if ((maxLength != -1) && (param.Length > maxLength)) {
				throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Error, the parameter {0} has exceeded the maximum length", paramName));
			}
		}

		/// <summary>Checks the password stored in the DB with the one provided.</summary>
		/// <remarks>This method is always called inside a transaction and updates the membership data about password attempts.</remarks>
		/// <param name="session">The session to use. There should be a transaction going on because this method uses automatic dirty checking.</param>
		/// <param name="username">The username.</param>
		/// <param name="password">The password.</param>
		/// <param name="failIfNotApproved">if set to true, the password will not match if the user is not approved.</param>
		/// <param name="mui">The MembershipUserInfo about the user that is going to have his password checked.</param>
		/// <returns>true if the password matchs; false otherwise.</returns>
		private bool CheckPassword(ISession session, string username, string password, bool failIfNotApproved, out MembershipUserInfo mui)
		{
			// get the membership data for this user
			MembershipUserInfoDal muiDAL = new MembershipUserInfoDal(session);
			mui = muiDAL.GetByUserName(username);

			if (mui == null) {
				return false;
			}

            //if (!mui.IsApproved && failIfNotApproved) {
            //    return false;
            //}

			if (mui.IsLockedOut && !EnableAutoUnlock) {
				return false;
			}

			// encode the password supplied and compare it to the one saved
			string encodedPassword = EncodePassword(password, mui.PasswordFormat, mui.PasswordSalt);
			bool passwordCorrect = mui.Password == encodedPassword;

			DateTime now = DateTime.UtcNow;

			// if the password was correct and there was any failed attempt, reset attempt data
			if (passwordCorrect) {
				// if the user is locked and the password is correct, try to auto unlock it (if that option is enabled)
				if ((mui.IsLockedOut && !AutoUnlockUser(mui)) /*|| !mui.IsApproved*/) {
					return false;
				}

				if ((mui.FailedPasswordAttemptCount > 0) || (mui.FailedPasswordAnswerAttemptCount > 0)) {
					mui.FailedPasswordAttemptCount = 0;
					mui.FailedPasswordAttemptWindowStart = null;
					mui.FailedPasswordAnswerAttemptCount = 0;
					mui.FailedPasswordAnswerAttemptWindowStart = null;
					mui.LastLockOutDate = null;
				}

				mui.LastActivityDate = now;
				mui.LastLoginDate = now;

				return true;
			} else {
				// if the current try has been done after the attempt window, init the attempts
				if (!mui.FailedPasswordAttemptWindowStart.HasValue || (mui.FailedPasswordAttemptWindowStart.Value.AddMinutes(PasswordAttemptWindow) <= now)) {
					mui.FailedPasswordAttemptCount = 1;
					mui.FailedPasswordAttemptWindowStart = now;
				} else {
					// otherwise, increment the number of attempts
					mui.FailedPasswordAttemptCount++;
					mui.FailedPasswordAttemptWindowStart = now;
				}

				// if the maximum number of attempts has been exceeded, lock the account
				if (mui.FailedPasswordAttemptCount >= MaxInvalidPasswordAttempts) {
					mui.IsLockedOut = true;
					mui.LastLockOutDate = now;
				}

				return false;
			}
		}

		/// <summary>Gets an encoded string using the encoding settings for an user.</summary>
		/// <remarks>This method is always called inside a transaction.</remarks>
		/// <param name="session">The session to use.</param>
		/// <param name="userName">The username to get the encoding settings.</param>
		/// <param name="valueToEncode">The string to encode.</param>
		/// <param name="mui">The MembershipUserInfo for the user.</param>
		/// <returns>The encoded string using the user settings.</returns>
		protected string GetEncodedStringByUserSettings(ISession session, string userName, string valueToEncode, out MembershipUserInfo mui)
		{
			// gets the membership data for this user
			MembershipUserInfoDal muiDAL = new MembershipUserInfoDal(session);
			mui = muiDAL.GetByUserName(userName);

			if (mui == null) {
				throw new ProviderException(String.Format(CultureInfo.CurrentCulture, "Error, userName {0} not found.", userName));
			}

			if (String.IsNullOrEmpty(valueToEncode)) {
				return valueToEncode;
			}

			// encodes the string using the current encoding for this user
			return EncodePassword(valueToEncode, mui.PasswordFormat, mui.PasswordSalt);
		}

		/// <summary>Checks if the password meets the defined policy requirements.</summary>
		/// <param name="password">The new password.</param>
		/// <returns>true if the password pass the policy; false otherwise.</returns>
		public virtual bool CheckPasswordPolicy(string password)
		{
			// password length check
			if (password.Length < MinRequiredPasswordLength) {
				return false;
			}

			int nonAlphanumericCharacters = 0;

			foreach (char c in password) {
				if (!Char.IsLetterOrDigit(c)) {
					nonAlphanumericCharacters++;
				}
			}

			// non alphanumerics check
			if (nonAlphanumericCharacters < MinRequiredNonAlphanumericCharacters) {
				return false;
			}

			// regular expression check
			if (!String.IsNullOrEmpty(PasswordStrengthRegularExpression)) {
				if (!Regex.IsMatch(password, PasswordStrengthRegularExpression)) {
					return false;
				}
			}

			return true;
		}

		/// <summary>Check if the password is validated, raising the ValidatingPassword event.</summary>
		/// <param name="userName">The username.</param>
		/// <param name="password">The password to validate.</param>
		/// <param name="args">The ValidatePasswordEventArgs instance containing the event data.</param>
		/// <returns>true if the password has passed validation; false otherwise.</returns>
		public virtual bool ValidatePassword(string userName, string password, out ValidatePasswordEventArgs args)
		{
			// throws the validating password event
			ValidatePasswordEventArgs e = new ValidatePasswordEventArgs(userName, password, false);
			OnValidatingPassword(e);

			args = e;
			return !e.Cancel;
		}

		/// <summary>Try to auto unlock a user.</summary>
		/// <param name="mui">The user data.</param>
		/// <returns>true if the account was locked and has been succesfully unlocked; false otherwise.</returns>
		protected virtual bool AutoUnlockUser(MembershipUserInfo mui)
		{
			if (EnableAutoUnlock && (mui != null) && (mui.IsLockedOut)) {
				DateTime now = DateTime.UtcNow;

				// if enough time has passed since the last lockout date, unlock the user
				if (mui.LastLockOutDate.HasValue) {
					if (mui.LastLockOutDate.Value.AddMinutes(AutoUnlockMinutes) <= now) {
						mui.FailedPasswordAttemptCount = 0;
						mui.FailedPasswordAttemptWindowStart = null;
						mui.FailedPasswordAnswerAttemptCount = 0;
						mui.FailedPasswordAnswerAttemptWindowStart = null;
						mui.LastLockOutDate = null;
						mui.IsLockedOut = false;

						return true;
					} else {
						return false;
					}
				}

				// if the last lockout date was not used, try with the failed password window
				if (mui.FailedPasswordAttemptWindowStart.HasValue) {
					if (mui.FailedPasswordAttemptWindowStart.Value.AddMinutes(AutoUnlockMinutes) <= now) {
						mui.FailedPasswordAttemptCount = 0;
						mui.FailedPasswordAttemptWindowStart = null;
						mui.FailedPasswordAnswerAttemptCount = 0;
						mui.FailedPasswordAnswerAttemptWindowStart = null;
						mui.LastLockOutDate = null;
						mui.IsLockedOut = false;

						return true;
					} else {
						return false;
					}
				}

				// if the last lockout date and the failed password window were not used, try with the failed password answer window
				if (mui.FailedPasswordAnswerAttemptWindowStart.HasValue) {
					if (mui.FailedPasswordAnswerAttemptWindowStart.Value.AddMinutes(AutoUnlockMinutes) <= now) {
						mui.FailedPasswordAttemptCount = 0;
						mui.FailedPasswordAttemptWindowStart = null;
						mui.FailedPasswordAnswerAttemptCount = 0;
						mui.FailedPasswordAnswerAttemptWindowStart = null;
						mui.LastLockOutDate = null;
						mui.IsLockedOut = false;

						return true;
					} else {
						return false;
					}
				}
			}

			// the account remains locked
			return false;
		}

		#endregion

		#endregion
	}
}