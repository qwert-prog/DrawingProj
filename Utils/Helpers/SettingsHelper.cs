using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace Utils.Helpers
{
    /// <summary>
    /// Class for reading/writing local application settings
    /// </summary>
    public class SettingsHelper
    {
        #region Private Properties

        /// <summary>
        /// Values ​​of the current local settings
        /// </summary>
        private static IPropertySet Settings => ApplicationData.Current.LocalSettings.Values;

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Reads a value from local settings by key. If there is no such key, it creates it with a default value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="defaultValue"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T ReadOrSetDefault<T>(T defaultValue, [CallerMemberName] string key = null)
        {
            if (Settings.TryGetValue(key, out object value))
            {
                return (T)value;
            }
            else
            {
                WriteSetting(defaultValue, key);
                return defaultValue;
            }
        }

        /// <summary>
        /// Writes the value to the key if it exists, creates and writes it if not.
        /// Works with <see cref="IPropertySet"/> local application settings
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void WriteSetting(object value, [CallerMemberName] string key = null)
        {
            if (!Settings.TryAdd(key, value))
            {
                Settings[key] = value;
            }
        }

        #endregion Public Methods
    }
}