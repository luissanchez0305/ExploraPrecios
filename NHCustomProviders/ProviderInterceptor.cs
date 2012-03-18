using System;
using System.Globalization;
using System.Web.Security;
using NHibernate;
using NHibernate.Type;

namespace NHCustomProviders
{
	/// <summary>An Interceptor to handle UTC DateTime values properly</summary>
	public class ProviderInterceptor : EmptyInterceptor
	{
		#region Fields

		private readonly MembershipPasswordFormat _defaultPasswordFormat;

		#endregion

		#region Properties

		/// <summary>Gets the default password format.</summary>
		protected MembershipPasswordFormat DefaultPasswordFormat
		{
			get { return _defaultPasswordFormat; }
		}

		#endregion

		#region Methods

		#region Constructor

		/// <summary>Initializes a new instance of the ProviderInterceptor class.</summary>
		/// <param name="defaultPasswordFormat">The default password format.</param>
		public ProviderInterceptor(MembershipPasswordFormat defaultPasswordFormat)
		{
			_defaultPasswordFormat = defaultPasswordFormat;
		}

		#endregion

		#region Method Overrides

		/// <summary>Called just before an object is initialized.</summary>
		/// <remarks>The interceptor may change the state, which will be propagated to the persistent object.
		/// When this method is called, entity will be an empty uninitialized instance of the class.</remarks>
		/// <param name="entity">The object to initialize.</param>
		/// <param name="id">The identifier of the entity.</param>
		/// <param name="state">The loaded state of the entity.</param>
		/// <param name="propertyNames">The properties for the entity.</param>
		/// <param name="types">The property types of the entity.</param>
		/// <returns>true if the user modified the state in any way.</returns>
		public override bool OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types)
		{
			// converts loaded dates to UTC format
			SetUtcDateTimeKind(state, types);

			// sets the default value for the password format
			MembershipUserInfo mui = entity as MembershipUserInfo;

			if (mui != null) {
				mui.PasswordFormat = DefaultPasswordFormat;
			}

			return true;
		}

		/// <summary>Called before an object is saved.</summary>
		/// <remarks>
		/// The interceptor may modify the state, which will be used for the SQL INSERT
		/// and propagated to the persistent object.
		/// </remarks>
		/// <param name="entity">The object to be saved.</param>
		/// <param name="id">The identifier of the entity.</param>
		/// <param name="state">The current state of the entity.</param>
		/// <param name="propertyNames">The properties for the entity.</param>
		/// <param name="types">The property types of the entity.</param>
		/// <returns>true if the user modified the state in any way.</returns>
		public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
		{
#if DEBUG
	// check that all dates are in UTC format
			CheckUtcDateTimeKind(null, state, propertyNames, types);
#endif
			return false;
		}

		/// <summary>Called when an object is detected to be dirty, during a flush.</summary>
		/// <remarks>The interceptor may modify the detected currentState, which will be propagated to
		/// both the database and the persistent object. All flushes end in an actual synchronization 
		/// with the database, in which as the new currentState will be propagated to the object, but 
		/// not necessarily (immediately) to the database. It is strongly recommended that the 
		/// interceptor not modify the previousState.</remarks>
		/// <param name="entity">The object to be flushed.</param>
		/// <param name="id">The identifier of the entity.</param>
		/// <param name="currentState">The current state of the entity.</param>
		/// <param name="previousState">The previous state of the entity.</param>
		/// <param name="propertyNames">The properties for the entity.</param>
		/// <param name="types">The property types of the entity.</param>
		/// <returns>true if the user modified the currentState in any way.</returns>
		public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
		{
#if DEBUG
	// check that all dates are in UTC format
			CheckUtcDateTimeKind(id, currentState, propertyNames, types);
#endif
			return false;
		}

		#endregion

		#region Helper Methods

		/// <summary>Sets any DateTime property to use UTC format.</summary>
		/// <param name="state">The current state of the entity.</param>
		/// <param name="types">The property types of the entity.</param>
		protected static void SetUtcDateTimeKind(object[] state, IType[] types)
		{
			for (int i = 0; i < types.Length; i++) {
				if (types[i].ReturnedClass == typeof(DateTime)) {
					if (state[i] != null) {
						state[i] = DateTime.SpecifyKind((DateTime)state[i], DateTimeKind.Utc);
					}
				}
			}
		}

		/// <summary>Checks that any DateTime property uses UTC format.</summary>
		/// <param name="id">The identifier of the entity.</param>
		/// <param name="state">The current state of the entity.</param>
		/// <param name="propertyNames">The properties for the entity.</param>
		/// <param name="types">The property types of the entity.</param>
		protected static void CheckUtcDateTimeKind(object id, object[] state, string[] propertyNames, IType[] types)
		{
			for (int i = 0; i < types.Length; i++) {
				if (types[i].ReturnedClass == typeof(DateTime)) {
					if (state[i] != null) {
						DateTime dateTime = (DateTime)state[i];

						if (dateTime.Kind != DateTimeKind.Utc) {
							throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture, "Error, DateTime is not UTC for property {0} [id = {1}]", propertyNames[i], (id != null) ? id.ToString() : "unsaved"));
						}
					}
				}
			}
		}

		#endregion

		#endregion
	}
}