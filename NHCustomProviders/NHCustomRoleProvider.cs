//===============================================================================
// NHCustomRoleProvider
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
//	* user names and role names are case sensitive.
//	* ApplicationName is ignored.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Web.Security;
using System.Xml;
using NHibernate;
using NHibernate.Cfg;

namespace NHCustomProviders
{
	/// <summary>A highly configurable custom role provider that uses NHibernate for DB access.</summary>
	public class NHCustomRoleProvider : RoleProvider
	{
		#region Fields

		private static ISessionFactory _usersSessionFactory;
		private static ISessionFactory _rolesSessionFactory;

		private string _applicationName;
		private string _configurationFile;

		private string _userTableName;
		private string _userIdColumn;
		private string _userIdType;
		private string _userIdGeneratorClass;
		private string _userNameColumn;
		private string _roleTableName;
		private string _roleIdColumn;
		private string _roleIdType;
		private string _roleIdGeneratorClass;
		private string _roleNameColumn;
		private string _joinTableName;

		private int _userNameColumnSize;
		private int _roleNameColumnSize;

		#endregion

		#region Properties

		/// <summary>Gets the session factory for the users.</summary>
		protected internal static ISessionFactory UsersSessionFactory
		{
			get { return _usersSessionFactory; }
		}

		/// <summary>Gets the session factory for the roles.</summary>
		protected internal static ISessionFactory RolesSessionFactory
		{
			get { return _rolesSessionFactory; }
		}

		/// <summary>The name of the application using the custom role provider.</summary>
		/// <remarks>This property is not persisted in the DB but maintained for compatibility reasons.</remarks>
		/// <returns>The name of the application using the custom role provider.</returns>
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

		#endregion

		#region Methods

		#region Init/End Methods

		/// <summary>Initializes the provider.</summary>
		/// <param name="name">The friendly name of the provider.</param>
		/// <param name="config">A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.</param>
		public override void Initialize(string name, NameValueCollection config)
		{
			if (config == null) {
				throw new ArgumentNullException("config");
			}

			if (String.IsNullOrEmpty(name)) {
				name = "NHCustomRoleProvider";
			}
			if (String.IsNullOrEmpty(config["description"])) {
				config.Add("description", "NHibernate Custom Role Provider (c) Manuel Abadia 2007");
			}

			base.Initialize(name, config);

			// try to get the provider configuration values

			_applicationName = config["applicationName"];
			config.Remove("applicationName");

			_configurationFile = config["configurationFile"];
			config.Remove("configurationFile");

			// try to get the table and column names for the user mapping
			_userTableName = ConfigurationHelper.GetString(config, "userTableName", null);
			config.Remove("userTableName");

			if (String.IsNullOrEmpty(_userTableName)) {
				throw new ProviderException("Error, the userTableName property must be provided");
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

			_userIdGeneratorClass = ConfigurationHelper.GetString(config, "userIdGeneratorClass", "native");
			config.Remove("userIdGeneratorClass");

			_userNameColumn = ConfigurationHelper.GetString(config, "userNameColumn", null);
			config.Remove("userNameColumn");

			if (String.IsNullOrEmpty(_userNameColumn)) {
				throw new ProviderException("Error, the userNameColumn property must be provided");
			}

			// try to get the table and column names for the role mapping
			_roleTableName = ConfigurationHelper.GetString(config, "roleTableName", null);
			config.Remove("roleTableName");

			if (String.IsNullOrEmpty(_roleTableName)) {
				throw new ProviderException("Error, the roleTableName property must be provided");
			}

			_roleIdColumn = ConfigurationHelper.GetString(config, "roleIdColumn", null);
			config.Remove("roleIdColumn");

			if (String.IsNullOrEmpty(_roleIdColumn)) {
				throw new ProviderException("Error, the roleIdColumn property must be provided");
			}

			_roleIdType = ConfigurationHelper.GetString(config, "roleIdType", "Int32");
			config.Remove("roleIdType");

			if (String.IsNullOrEmpty(_roleIdType)) {
				throw new ProviderException("Error, the roleIdType property must be provided");
			}

			_roleIdGeneratorClass = ConfigurationHelper.GetString(config, "roleIdGeneratorClass", "native");
			config.Remove("roleIdGeneratorClass");

			_roleNameColumn = ConfigurationHelper.GetString(config, "roleNameColumn", null);
			config.Remove("roleNameColumn");

			if (String.IsNullOrEmpty(_roleNameColumn)) {
				throw new ProviderException("Error, the roleNameColumn property must be provided");
			}

			// get the join table name
			_joinTableName = ConfigurationHelper.GetString(config, "joinTableName", null);
			config.Remove("joinTableName");

			if (String.IsNullOrEmpty(_joinTableName)) {
				throw new ProviderException("Error, the joinTableName property must be provided");
			}

			// try to get the size of the columns
			_userNameColumnSize = ConfigurationHelper.GetInt32(config, "userNameColumnSize", -1, 1, Int32.MaxValue);
			config.Remove("userNameColumnSize");

			_roleNameColumnSize = ConfigurationHelper.GetInt32(config, "roleNameColumnSize", -1, 1, Int32.MaxValue);
			config.Remove("roleNameColumnSize");

			// if there is any parameter not processed, error!
			if (config.Count > 0) {
				string nonProcessedAttributte = config.GetKey(0);
				if (!String.IsNullOrEmpty(nonProcessedAttributte)) {
					throw new ProviderException(String.Format(CultureInfo.CurrentCulture, "Unrecognized attribute {0} found in the provider settings", nonProcessedAttributte));
				}
			}

			// Generate two mappings based on the configuration properties
			Configuration cfig1, cfig2;
			ConfigureNHibernate(out cfig1, out cfig2);

			// builds the session factories
			_usersSessionFactory = cfig1.BuildSessionFactory();
			_rolesSessionFactory = cfig2.BuildSessionFactory();
		}

		/// <summary>Dispose resources used by the session factories.</summary>
		~NHCustomRoleProvider()
		{
			if (_usersSessionFactory != null) {
				_usersSessionFactory.Close();
			}

			if (_rolesSessionFactory != null) {
				_rolesSessionFactory.Close();
			}
		}

		#endregion

		#region NHibernate related Methods

		/// <summary>Creates the configuration objects with the settings to use NHibernate.</summary>
		/// <param name="cfig1">First configuration object created.</param>
		/// <param name="cfig2">Second configuration object created.</param>
		protected virtual void ConfigureNHibernate(out Configuration cfig1, out Configuration cfig2)
		{
			bool userIsInverse = true;
			bool roleIsInverse = false;

			cfig1 = new Configuration();
			cfig2 = new Configuration();

			if (String.IsNullOrEmpty(ConfigurationFile)) {
				cfig1.Configure();
				cfig2.Configure();
			} else {
				string baseDir = AppDomain.CurrentDomain.BaseDirectory;
				string relativeSearchPath = AppDomain.CurrentDomain.RelativeSearchPath;
				string binPath = relativeSearchPath == null ? baseDir : Path.Combine(baseDir, relativeSearchPath);
				string configFile = Path.Combine(binPath, ConfigurationFile);
				cfig1.Configure(configFile);
				cfig2.Configure(configFile);
			}

			// generates the mapping information for roles and users
			RoleClassMappingInfo mapInfoUsers = new RoleClassMappingInfo();
			mapInfoUsers.ClassName = "BasicUser";
			mapInfoUsers.TableName = _userTableName;
			mapInfoUsers.IdPropertyName = "UserId";
			mapInfoUsers.IdColumnName = _userIdColumn;
			mapInfoUsers.IdColumnType = _userIdType;
			mapInfoUsers.GeneratorStrategy = _userIdGeneratorClass;
			mapInfoUsers.PropName = "UserName";
			mapInfoUsers.PropColumnName = _userNameColumn;
			mapInfoUsers.PropUnique = true;
			mapInfoUsers.CollectionPropertyName = "Roles";
			mapInfoUsers.JoinTableName = _joinTableName;
			mapInfoUsers.Inverse = roleIsInverse;
			mapInfoUsers.AssociatedColumnName = _roleIdColumn;
			mapInfoUsers.AssociatedClassName = "BasicRole";

			RoleClassMappingInfo mapInfoRoles = new RoleClassMappingInfo();
			mapInfoRoles.ClassName = "BasicRole";
			mapInfoRoles.TableName = _roleTableName;
			mapInfoRoles.IdPropertyName = "RoleId";
			mapInfoRoles.IdColumnName = _roleIdColumn;
			mapInfoRoles.IdColumnType = _roleIdType;
			mapInfoRoles.GeneratorStrategy = _roleIdGeneratorClass;
			mapInfoRoles.PropName = "RoleName";
			mapInfoRoles.PropColumnName = _roleNameColumn;
			mapInfoRoles.PropUnique = true;
			mapInfoRoles.CollectionPropertyName = "Users";
			mapInfoRoles.JoinTableName = _joinTableName;
			mapInfoRoles.Inverse = userIsInverse;
			mapInfoRoles.AssociatedColumnName = _userIdColumn;
			mapInfoRoles.AssociatedClassName = "BasicUser";

			// creates the users/roles mapping with the inverse relation in one side
			XmlDocument doc1 = GenerateClassMapping(mapInfoUsers);
			XmlDocument doc2 = GenerateClassMapping(mapInfoRoles);

			cfig1.AddDocument(doc1);
			cfig1.AddDocument(doc2);

			// change the inverse property
			userIsInverse = !userIsInverse;
			roleIsInverse = !roleIsInverse;
			mapInfoRoles.Inverse = userIsInverse;
			mapInfoUsers.Inverse = roleIsInverse;

			// creates the users/roles mapping with the inverse relation in the other side
			XmlDocument doc3 = GenerateClassMapping(mapInfoUsers);
			XmlDocument doc4 = GenerateClassMapping(mapInfoRoles);

			cfig2.AddDocument(doc3);
			cfig2.AddDocument(doc4);
		}

		/// <summary>Generates the class mapping for a Role entity.</summary>
		/// <param name="mapInfo">Information about the mapping for the role provider.</param>
		/// <returns>An Xml document with the class mapping.</returns>
		[SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
		[SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "We are not normalizing strings")]
		protected static XmlDocument GenerateClassMapping(RoleClassMappingInfo mapInfo)
		{
			XmlElement classNode;

			XmlDocument doc = XmlDocHelper.CreateClassMappingDocument(mapInfo.ClassName, mapInfo.TableName, out classNode);

			IDictionary<string, string> attributes = new Dictionary<string, string>();

			// creates the id and generator
			attributes.Clear();
			attributes["name"] = mapInfo.IdPropertyName;
			attributes["column"] = mapInfo.IdColumnName;
			attributes["type"] = mapInfo.IdColumnType;
			XmlElement idNode = XmlDocHelper.CreateElementWithAttributes(doc, "id", attributes);

			attributes.Clear();
			attributes["class"] = mapInfo.GeneratorStrategy;
			XmlElement generatorNode = XmlDocHelper.CreateElementWithAttributes(doc, "generator", attributes);
			idNode.AppendChild(generatorNode);
			classNode.AppendChild(idNode);

			// creates the name property
			attributes.Clear();
			attributes["name"] = mapInfo.PropName;
			attributes["column"] = mapInfo.PropColumnName;
			attributes["type"] = "String";
			attributes["not-null"] = "true";
			attributes["unique"] = mapInfo.PropUnique.ToString().ToLower(CultureInfo.InvariantCulture);
			XmlElement propertyNode = XmlDocHelper.CreateElementWithAttributes(doc, "property", attributes);
			classNode.AppendChild(propertyNode);

			// creates the collection property for the many-to-many relationship
			attributes.Clear();
			attributes["name"] = mapInfo.CollectionPropertyName;
			attributes["table"] = mapInfo.JoinTableName;
			attributes["cascade"] = "save-update";
			attributes["fetch"] = "subselect";
			attributes["inverse"] = mapInfo.Inverse.ToString().ToLower(CultureInfo.InvariantCulture);
			XmlElement setNode = XmlDocHelper.CreateElementWithAttributes(doc, "set", attributes);

			attributes.Clear();
			attributes["column"] = mapInfo.IdColumnName;
			XmlElement keyNode = XmlDocHelper.CreateElementWithAttributes(doc, "key", attributes);
			setNode.AppendChild(keyNode);

			attributes.Clear();
			attributes["class"] = mapInfo.AssociatedClassName;
			attributes["column"] = mapInfo.AssociatedColumnName;
			XmlElement manyNode = XmlDocHelper.CreateElementWithAttributes(doc, "many-to-many", attributes);
			setNode.AppendChild(manyNode);

			classNode.AppendChild(setNode);

			return doc;
		}

		#endregion

		#region Role Assignment

		/// <summary>Adds the specified user names to the specified roles.</summary>
		/// <param name="usernames">A string array of user names to be added to the specified roles.</param>
		/// <param name="roleNames">A string array of the role names to add the specified user names to.</param>
		public override void AddUsersToRoles(string[] usernames, string[] roleNames)
		{
			// executes the following code inside a transaction
			Inside.Transaction(_usersSessionFactory, delegate(ISession session, ITransaction trans) {
			                                         	RolesDal rolesDAL = new RolesDal(session);

			                                         	// check that the role names are correct
			                                         	for (int i = 0; i < roleNames.Length; i++) {
			                                         		string roleName = roleNames[i];

			                                         		// check if the role name is valid
			                                         		CheckParameter(roleName, "roleName", true, true, _roleNameColumnSize);
			                                         	}

			                                         	// get all roles
			                                         	IList<BasicRole> roles = rolesDAL.GetRoles(roleNames);

			                                         	// if not all roles were found, throw an error
			                                         	if (roles.Count != roleNames.Length) {
			                                         		for (int i = 0; i < roleNames.Length; i++) {
			                                         			string roleName = roleNames[i];
			                                         			bool found = false;

			                                         			foreach (BasicRole role in roles) {
			                                         				if (role.RoleName == roleName) {
			                                         					found = true;
			                                         					break;
			                                         				}
			                                         			}

			                                         			if (!found) {
			                                         				throw new ProviderException(String.Format(CultureInfo.CurrentCulture, "Error, the role {0} was not found", roleName));
			                                         			}
			                                         		}
			                                         	}

			                                         	// iterate through the users, adding each role to them
			                                         	foreach (string username in usernames) {
			                                         		// check if the user name is valid
			                                         		CheckParameter(username, "username", true, true, _userNameColumnSize);

			                                         		BasicUser user = rolesDAL.GetUserByName(username);

			                                         		if (user == null) {
			                                         			throw new ProviderException(String.Format(CultureInfo.CurrentCulture, "Error, the user {0} was not found", username));
			                                         		}

			                                         		// add the roles to the user
			                                         		foreach (BasicRole role in roles) {
			                                         			user.Roles.Add(role);
			                                         		}
			                                         	}
			                                         });
		}

		/// <summary>Removes the specified user names from the specified roles.</summary>
		/// <param name="usernames">A string array of user names to be removed from the specified roles.</param>
		/// <param name="roleNames">A string array of role names to remove the specified user names from.</param>
		public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
		{
			// executes the following code inside a transaction
			Inside.Transaction(_usersSessionFactory, delegate(ISession session, ITransaction trans) {
			                                         	RolesDal rolesDAL = new RolesDal(session);

			                                         	// check that the role names are correct
			                                         	for (int i = 0; i < roleNames.Length; i++) {
			                                         		string roleName = roleNames[i];

			                                         		// check if the role name is valid
			                                         		CheckParameter(roleName, "roleName", true, true, _roleNameColumnSize);
			                                         	}

			                                         	// get all roles
			                                         	IList<BasicRole> roles = rolesDAL.GetRoles(roleNames);

			                                         	// if not all roles were found, throw an error
			                                         	if (roles.Count != roleNames.Length) {
			                                         		for (int i = 0; i < roleNames.Length; i++) {
			                                         			string roleName = roleNames[i];
			                                         			bool found = false;

			                                         			foreach (BasicRole role in roles) {
			                                         				if (role.RoleName == roleName) {
			                                         					found = true;
			                                         					break;
			                                         				}
			                                         			}

			                                         			if (!found) {
			                                         				throw new ProviderException(String.Format(CultureInfo.CurrentCulture, "Error, the role {0} was not found", roleName));
			                                         			}
			                                         		}
			                                         	}

			                                         	// iterate through the users, removing each role from them
			                                         	foreach (string username in usernames) {
			                                         		// check if the user name is valid
			                                         		CheckParameter(username, "username", true, true, _userNameColumnSize);

			                                         		BasicUser user = rolesDAL.GetUserByName(username);

			                                         		if (user == null) {
			                                         			throw new ProviderException(String.Format(CultureInfo.CurrentCulture, "Error, the user {0} was not found", username));
			                                         		}

			                                         		// add the roles to the user
			                                         		foreach (BasicRole role in roles) {
			                                         			user.Roles.Remove(role);
			                                         		}
			                                         	}
			                                         });
		}

		#endregion

		#region Role Creation/Deletion

		/// <summary>Adds a new role to the DB.</summary>
		/// <param name="roleName">The name of the role to create.</param>
		public override void CreateRole(string roleName)
		{
			// check if the parameters are valid
			CheckParameter(roleName, "roleName", true, true, _roleNameColumnSize);

			// executes the following code inside a transaction
			Inside.Transaction(_rolesSessionFactory, delegate(ISession session, ITransaction trans) {
			                                         	RolesDal rolesDAL = new RolesDal(session);

			                                         	// try to get a role with that name
			                                         	BasicRole role = rolesDAL.GetRoleByName(roleName);

			                                         	if (role != null) {
			                                         		throw new ProviderException(String.Format(CultureInfo.CurrentCulture, "Error, the role {0} already exists", roleName));
			                                         	}

			                                         	// if it doesn't exists, create it
			                                         	role = new BasicRole();
			                                         	role.RoleName = roleName;

			                                         	session.Save(role);
			                                         });
		}

		/// <summary>Removes a role from the DB.</summary>
		/// <param name="roleName">The name of the role to delete.</param>
		/// <param name="throwOnPopulatedRole">If true, throw an exception if roleName has one or more members and do not delete roleName.</param>
		/// <returns>true if the role was successfully deleted; otherwise, false.</returns>
		public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
		{
			// check if the parameters are valid
			CheckParameter(roleName, "roleName", true, true, _roleNameColumnSize);

			// executes the following code inside a transaction
			return Inside.Transaction<bool>(_rolesSessionFactory, delegate(ISession session, ITransaction trans) {
			                                                      	RolesDal rolesDAL = new RolesDal(session);

			                                                      	// try to get a role with that name
			                                                      	BasicRole role = rolesDAL.GetRoleByName(roleName);

			                                                      	if (role == null) {
			                                                      		return false;
			                                                      	}

			                                                      	if (throwOnPopulatedRole) {
			                                                      		if (role.Users.Count > 0) {
			                                                      			throw new ProviderException(String.Format(CultureInfo.CurrentCulture, "Error, can not delete role {0} because there are users with that role", roleName));
			                                                      		}
			                                                      	}

			                                                      	// deletes the specified role and the users in role entries associated to this role
			                                                      	return rolesDAL.Delete(role);
			                                                      });
		}

		#endregion

		#region Queries

		/// <summary>Gets a value indicating whether the specified user is in the specified role.</summary>
		/// <param name="username">The user name to search for.</param>
		/// <param name="roleName">The role to search in.</param>
		/// <returns>true if the specified user is in the specified role; otherwise, false.</returns>
		public override bool IsUserInRole(string username, string roleName)
		{
			// check if the parameters are valid
			CheckParameter(username, "username", true, true, _userNameColumnSize);
			CheckParameter(roleName, "roleName", true, true, _roleNameColumnSize);

			// executes the following code inside a transaction
			return Inside.Transaction<bool>(_usersSessionFactory, delegate(ISession session, ITransaction trans) {
			                                                      	RolesDal rolesDAL = new RolesDal(session);

			                                                      	// try to get an user with that name
			                                                      	BasicUser user = rolesDAL.GetUserByName(username);

			                                                      	if (user == null) {
			                                                      		return false;
			                                                      	}

			                                                      	// searchs the role in the user
			                                                      	foreach (BasicRole role in user.Roles) {
			                                                      		if (String.Compare(role.RoleName, roleName, StringComparison.OrdinalIgnoreCase) == 0) {
			                                                      			return true;
			                                                      		}
			                                                      	}

			                                                      	return false;
			                                                      });
		}

		/// <summary>Gets a value indicating whether the specified role name already exists in the DB.</summary>
		/// <param name="roleName">The name of the role to search for.</param>
		/// <returns>true if the role name already exists in the DB; otherwise, false.</returns>
		public override bool RoleExists(string roleName)
		{
			// check if the parameters are valid
			CheckParameter(roleName, "roleName", true, true, _roleNameColumnSize);

			// executes the following code inside a transaction
			return Inside.Transaction<bool>(_rolesSessionFactory, delegate(ISession session, ITransaction trans) {
			                                                      	RolesDal rolesDAL = new RolesDal(session);

			                                                      	// try to get a role with that name
			                                                      	BasicRole role = rolesDAL.GetRoleByName(roleName);

			                                                      	return role != null;
			                                                      });
		}

		/// <summary>Gets a list of all the roles.</summary>
		/// <returns>A string array containing the names of all the roles stored in the DB.</returns>
		public override string[] GetAllRoles()
		{
			// executes the following code inside a transaction
			return Inside.Transaction<string[]>(_rolesSessionFactory, delegate(ISession session, ITransaction trans) {
			                                                          	RolesDal rolesDAL = new RolesDal(session);

			                                                          	// get all roles
			                                                          	return rolesDAL.GetAllRoles();
			                                                          });
		}

		/// <summary>Gets a list of the roles that a specified user is in.</summary>
		/// <param name="username">The user to return a list of roles for.</param>
		/// <returns>A string array containing the names of all the roles that the specified user is in.</returns>
		public override string[] GetRolesForUser(string username)
		{
			// check if the parameters are valid
			CheckParameter(username, "username", true, true, _userNameColumnSize);

			// executes the following code inside a transaction
			return Inside.Transaction<string[]>(_usersSessionFactory, delegate(ISession session, ITransaction trans) {
			                                                          	RolesDal rolesDAL = new RolesDal(session);

			                                                          	// get the user
			                                                          	BasicUser user = rolesDAL.GetUserByName(username);

			                                                          	if (user == null) {
			                                                          		return new string[0];
			                                                          	}

			                                                          	// if the user exists, collect the role names
			                                                          	string[] roles = new string[user.Roles.Count];

			                                                          	int i = 0;
			                                                          	foreach (BasicRole role in user.Roles) {
			                                                          		roles[i] = role.RoleName;
			                                                          		i++;
			                                                          	}

			                                                          	return roles;
			                                                          });
		}

		/// <summary>Gets a list of users in the specified role.</summary>
		/// <param name="roleName">The name of the role to get the list of users for.</param>
		/// <returns>A string array containing the names of all the users who are members of the specified role.</returns>
		public override string[] GetUsersInRole(string roleName)
		{
			// check if the parameters are valid
			CheckParameter(roleName, "roleName", true, true, _roleNameColumnSize);

			// executes the following code inside a transaction
			return Inside.Transaction<string[]>(_usersSessionFactory, delegate(ISession session, ITransaction trans) {
			                                                          	RolesDal rolesDAL = new RolesDal(session);

			                                                          	// get the role
			                                                          	BasicRole role = rolesDAL.GetRoleByName(roleName);

			                                                          	if (role == null) {
			                                                          		return new string[0];
			                                                          	}

			                                                          	// if the role exists, collect the user names
			                                                          	string[] userNames = new string[role.Users.Count];

			                                                          	int i = 0;
			                                                          	foreach (BasicUser user in role.Users) {
			                                                          		userNames[i] = user.UserName;
			                                                          		i++;
			                                                          	}

			                                                          	return userNames;
			                                                          });
		}

		/// <summary>Gets an array of user names in a role where the user name contains the specified user name to match.</summary>
		/// <param name="roleName">The role to search in.</param>
		/// <param name="usernameToMatch">The user name to search for.</param>
		/// <returns>A string array containing the names of all the users where the user name matches usernameToMatch and the user is a member of the specified role.</returns>
		public override string[] FindUsersInRole(string roleName, string usernameToMatch)
		{
			// check if the parameters are valid
			CheckParameter(roleName, "roleName", true, true, _roleNameColumnSize);
			CheckParameter(usernameToMatch, "usernameToMatch", true, true, _userNameColumnSize);

			// executes the following code inside a transaction
			return Inside.Transaction<string[]>(_usersSessionFactory, delegate(ISession session, ITransaction trans) {
			                                                          	RolesDal rolesDAL = new RolesDal(session);

			                                                          	return rolesDAL.FindUsersInRole(roleName, usernameToMatch);
			                                                          });
		}

		#endregion

		#region Helper Methods

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

		#endregion

		#endregion
	}
}