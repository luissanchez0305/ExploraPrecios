using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Web;
using System.Web.Management;

namespace NHCustomProviders
{
	/// <summary>Helper class to make WebEvents usable.</summary>
	/// <remarks>This is necessary because in ASP.NET 2.0 none of the WebEvents can be created and 
	/// there is no option to throw any system level events. What were they thinking about?</remarks>
	[AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public static class WebEventsHelper
	{
		#region Fields

		// object used for access synchronization
		private static readonly object _methodLock = new Object();

		private static MethodInfo _raiseSystemEvent;

		#endregion

		#region Methods

		/// <summary>Raises a system event.</summary>
		/// <param name="message">The event description.</param>
		/// <param name="source">The object that is the source of the event.</param>
		/// <param name="eventCode">The event code.</param>
		/// <param name="eventDetailCode">The event detail code.</param>
		/// <param name="exception">The exception that triggered the event.</param>
		/// <param name="nameToAuthenticate">The name of the authenticated user.</param>
		[ReflectionPermission(SecurityAction.Assert, Unrestricted = true), SecurityCritical, SecurityTreatAsSafe]
		[SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
		public static void RaiseSystemEvent(string message, object source, int eventCode, int eventDetailCode, Exception exception, string nameToAuthenticate)
		{
			// in the first call, get the method
			if (_raiseSystemEvent == null) {
				lock (_methodLock) {
					if (_raiseSystemEvent == null) {
						_raiseSystemEvent = typeof(WebBaseEvent).GetMethod("RaiseSystemEventInternal", BindingFlags.Static | BindingFlags.NonPublic);
					}
				}
			}

			// call the private method
			_raiseSystemEvent.Invoke(null, new object[] { message, source, eventCode, eventDetailCode, exception, nameToAuthenticate });
		}

		#endregion
	}
}