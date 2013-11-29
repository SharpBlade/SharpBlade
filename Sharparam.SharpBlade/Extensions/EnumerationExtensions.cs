// Code for EnumExtensions class written by Hugoware
// http://hugoware.net/blog/enumeration-extensions-2-0
// Modifications done by Sharparam: ReSharper cleanup

using System;

namespace Sharparam.SharpBlade.Extensions
{
    /// <summary>
    /// Extension methods to make working with enumeration values easier
    /// </summary>
    public static class EnumerationExtensions
    {
        #region Extension Methods

        /// <summary>
        /// Includes an enumerated type and returns the new value
        /// </summary>
        /// <typeparam name="T">The type of the value(s) being appended.</typeparam>
        /// <param name="value">The <see cref="Enum" /> to add values to.</param>
        /// <param name="append">The value(s) to append.</param>
        /// <returns>A new enumeration of type <see cref="T" /> with the
        /// value(s) in the parameter append included.</returns>
        public static T Include<T>(this Enum value, T append)
        {
            var type = value.GetType();

            // determine the values
            object result = value;
            var parsed = new _Value(append, type);
            if (parsed.Signed.HasValue)
            {
                result = Convert.ToInt64(value) | (long)parsed.Signed;
            }
            else if (parsed.Unsigned.HasValue)
            {
                result = Convert.ToUInt64(value) | (ulong)parsed.Unsigned;
            }

            // return the final value
            return (T)Enum.Parse(type, result.ToString());
        }

        /// <summary>
        /// Removes an enumerated type and returns the new value
        /// </summary>
        /// <typeparam name="T">The type of the value(s) being removed.</typeparam>
        /// <param name="value">The <see cref="Enum" /> to remove values from.</param>
        /// <param name="remove">The value(s) to remove from the enumeration.</param>
        /// <returns>A new enumeration of type <see cref="T" /> with the value(s)
        /// supplied in <see cref="remove" /> removed.</returns>
        public static T Remove<T>(this Enum value, T remove)
        {
            Type type = value.GetType();

            // determine the values
            object result = value;
            var parsed = new _Value(remove, type);
            if (parsed.Signed.HasValue)
            {
                result = Convert.ToInt64(value) & ~(long)parsed.Signed;
            }
            else if (parsed.Unsigned.HasValue)
            {
                result = Convert.ToUInt64(value) & ~(ulong)parsed.Unsigned;
            }

            // return the final value
            return (T)Enum.Parse(type, result.ToString());
        }

        /// <summary>
        /// Checks if an enumerated type contains a value
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="check" /> parameter.</typeparam>
        /// <param name="value">The enumeration value to check.</param>
        /// <param name="check">The value(s) to test for.</param>
        /// <returns>True if <see cref="value" /> contains all values
        /// in <see cref="check" />, false otherwise.</returns>
        public static bool Has<T>(this Enum value, T check)
        {
            var type = value.GetType();

            // determine the values
            var parsed = new _Value(check, type);

            if (parsed.Signed.HasValue)
                return (Convert.ToInt64(value) & (long)parsed.Signed) == (long)parsed.Signed;

            if (parsed.Unsigned.HasValue)
                return (Convert.ToUInt64(value) & (ulong)parsed.Unsigned) == (ulong)parsed.Unsigned;

            return false;
        }

        /// <summary>
        /// Checks if an enumerated type is missing a value
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="value" /> parameter.</typeparam>
        /// <param name="obj">The enumeration value to check.</param>
        /// <param name="value">The value(s) to test for.</param>
        /// <returns>True if <see cref="obj" /> is missing all values
        /// in <see cref="value" />, false otherwise.</returns>
        public static bool Missing<T>(this Enum obj, T value)
        {
            return !Has(obj, value);
        }

        #endregion

        #region Helper Classes

        /// <summary>
        /// Class to simplify narrowing values between
        /// a unsigned long and long since either value should
        /// cover any lesser value.
        /// </summary>
        private class _Value
        {
            /// <summary>
            /// Signed value (or null).
            /// </summary>
            public readonly long? Signed;

            /// <summary>
            /// Unsigned value (or null).
            /// </summary>
            public readonly ulong? Unsigned;

            // cached comparisons for tye to use

            /// <summary>
            /// Cached comparison variable for unsigned 64bit integers.
            /// </summary>
            private static readonly Type CachedUInt64 = typeof(ulong);

            /// <summary>
            /// Cached comparison variable for unsigned 32bit integers.
            /// </summary>
            private static readonly Type CachedUint32 = typeof(long);

            /// <summary>
            /// Initializes a new instance of the <see cref="_Value" /> class.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="type">The type of <see cref="value" />.</param>
            public _Value(object value, Type type)
            {
                // make sure it is even an enum to work with
                if (!type.IsEnum)
                    throw new ArgumentException("Value provided is not an enumerated type!");

                // then check for the enumerated value
                var compare = Enum.GetUnderlyingType(type);

                // if this is an unsigned long then the only
                // value that can hold it would be a ulong
                if (compare == CachedUint32 || compare == CachedUInt64)
                    Unsigned = Convert.ToUInt64(value);
                else // otherwise, a long should cover anything else
                    Signed = Convert.ToInt64(value);
            }
        }

        #endregion
    }
}
