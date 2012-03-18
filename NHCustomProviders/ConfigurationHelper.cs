using System;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Globalization;

namespace NHCustomProviders
{
	/// <summary>Helper methods to extract data from the configuration.</summary>
	public static class ConfigurationHelper
	{
		#region Methods

		/// <summary>Gets a boolean value from the configuration options.</summary>
		/// <param name="config">The list of configuration parameters.</param>
		/// <param name="propertyName">Name of the property to get.</param>
		/// <param name="defaultValue">Value to return if the property is not set.</param>
		/// <returns>The value of the property in the configuration parameters, or the defaultValue if the property is not set.</returns>
		public static bool GetBoolean(NameValueCollection config, string propertyName, bool defaultValue)
		{
			string propertyValue = config[propertyName];

			if (propertyValue == null) {
				return defaultValue;
			} else {
				bool result;

				if (Boolean.TryParse(propertyValue, out result)) {
					return result;
				} else {
					throw new ProviderException(String.Format(CultureInfo.CurrentCulture, "The value must be true or false for the property '{0}'", propertyName));
				}
			}
		}

		/// <summary>Gets an integer value from the configuration options.</summary>
		/// <param name="config">The list of configuration parameters.</param>
		/// <param name="propertyName">Name of the property to get.</param>
		/// <param name="defaultValue">Value to return if the property is not set.</param>
		/// <param name="minValueAllowed">The min value allowed.</param>
		/// <param name="maxValueAllowed">The max value allowed.</param>
		/// <returns>The value of the property in the configuration parameters, or the defaultValue if the property is not set.</returns>
		public static int GetInt32(NameValueCollection config, string propertyName, int defaultValue, int minValueAllowed, int maxValueAllowed)
		{
			string propertyValue = config[propertyName];

			if (propertyValue == null) {
				return defaultValue;
			} else {
				int result;

				if (Int32.TryParse(propertyValue, out result)) {
					if ((result < minValueAllowed) || (result > maxValueAllowed)) {
						throw new ProviderException(String.Format(CultureInfo.CurrentCulture, "The value for the property '{0}' is not in the accepted range [{1}, {2}]", propertyName, minValueAllowed, maxValueAllowed));
					}

					return result;
				} else {
					throw new ProviderException(String.Format(CultureInfo.CurrentCulture, "The value must be integer for the property '{0}'", propertyName));
				}
			}
		}

		/// <summary>Gets a string value from the configuration options.</summary>
		/// <param name="config">The list of configuration parameters.</param>
		/// <param name="propertyName">Name of the property to get.</param>
		/// <param name="defaultValue">Value to return if the property is not set.</param>
		/// <returns>The value of the property in the configuration parameters, or the defaultValue if the property is not set.</returns>
		public static string GetString(NameValueCollection config, string propertyName, string defaultValue)
		{
			string propertyValue = config[propertyName];

			if (propertyValue == null) {
				return defaultValue;
			} else {
				return propertyValue.Trim();
			}
		}

		/// <summary>Gets an enum value from the configuration options.</summary>
		/// <param name="config">The list of configuration parameters.</param>
		/// <param name="propertyName">Name of the property to get.</param>
		/// <param name="enumType">the type of the enum we're retrieving.</param>
		/// <param name="defaultValue">Value to return if the property is not set.</param>
		/// <returns>The value of the property in the configuration parameters, or the defaultValue if the property is not set.</returns>
		public static int GetEnum(NameValueCollection config, string propertyName, Type enumType, int defaultValue)
		{
			string propertyValue = config[propertyName];

			if (propertyValue == null) {
				return defaultValue;
			} else {
				try {
					return (int)Enum.Parse(enumType, propertyValue, false);
				} catch {
					throw new ProviderException(String.Format(CultureInfo.CurrentCulture, "Error, the value '{0}' is not a supported value for the '{1}' enum", propertyValue, enumType.FullName));
				}
			}
		}

		#endregion
	}
}