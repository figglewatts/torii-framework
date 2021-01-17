using System;
using System.Collections.Generic;
using System.Linq;

namespace Torii.Util
{
    /// <summary>
    /// Various utility methods for Enums.
    /// </summary>
    public static class EnumUtil
    {
        /// <summary>
        /// Parse a string value to an enum. Case insensitive.
        /// </summary>
        /// <typeparam name="T">The Type of the enum</typeparam>
        /// <param name="value">The string value to parse</param>
        /// <returns>The parsed enum value</returns>
        public static T Parse<T>(string value) where T : Enum { return (T)Enum.Parse(typeof(T), value, true); }

        /// <summary>
        /// Try to parse a string value to an enum. Case insensitive.
        /// </summary>
        /// <typeparam name="T">The Type of the enum.</typeparam>
        /// <param name="value">The string value to parse to.</param>
        /// <param name="parsed">The parsed enum. If the enum could not be parsed then this value is the default value of T.</param>
        /// <returns>True if the parse succeeded, false otherwise.</returns>
        public static bool TryParse<T>(string value, out T parsed) where T : Enum
        {
            try
            {
                parsed = (T)Enum.Parse(typeof(T), value, true);
                return true;
            }
            catch (ArgumentException)
            {
                parsed = default(T);
                return false;
            }
        }

        /// <summary>
        /// Get all the values of an enum type.
        /// </summary>
        /// <typeparam name="T">The enum type.</typeparam>
        /// <returns>An enumerable containing all possible enum values.</returns>
        public static IEnumerable<T> GetValues<T>() where T : Enum { return Enum.GetValues(typeof(T)).Cast<T>(); }
    }
}
