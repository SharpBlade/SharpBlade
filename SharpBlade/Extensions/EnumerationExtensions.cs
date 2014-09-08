// ---------------------------------------------------------------------------------------
// <copyright file="EnumerationExtensions.cs" company="SharpBlade">
//     Copyright © 2013-2014 by Adam Hellberg and Brandon Scott.
//
//     Permission is hereby granted, free of charge, to any person obtaining a copy of
//     this software and associated documentation files (the "Software"), to deal in
//     the Software without restriction, including without limitation the rights to
//     use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
//     of the Software, and to permit persons to whom the Software is furnished to do
//     so, subject to the following conditions:
//
//     The above copyright notice and this permission notice shall be included in all
//     copies or substantial portions of the Software.
//
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
//     WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//     CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//     Disclaimer: SharpBlade is in no way affiliated with Razer and/or any of
//     its employees and/or licensors. Adam Hellberg and/or Brandon Scott do not
//     take responsibility for any harm caused, direct or indirect, to any Razer
//     peripherals via the use of SharpBlade.
//
//     "Razer" is a trademark of Razer USA Ltd.
// </copyright>
// ---------------------------------------------------------------------------------------

namespace SharpBlade.Extensions
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Extension methods to make working with enumeration values easier
    /// </summary>
    public static class EnumerationExtensions
    {
        /// <summary>
        /// Checks if an enumerated type contains a value
        /// </summary>
        /// <typeparam name="T">The type of the <c>check</c> parameter.</typeparam>
        /// <param name="value">The enumeration value to check.</param>
        /// <param name="check">The value(s) to test for.</param>
        /// <returns>True if <c>value</c> contains all values
        /// in <c>check</c>, false otherwise.</returns>
        public static bool Has<T>(this Enum value, T check)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            var type = value.GetType();

            var checkType = typeof(T);

            if (checkType != type)
                throw new ArgumentException("The enums have to be of the same type", "check");

            // determine the values
            var parsed = new Value(check, type);

            var result = false;

            if (parsed.Signed.HasValue)
            {
                result = (Convert.ToInt64(value, CultureInfo.InvariantCulture) & (long)parsed.Signed)
                       == (long)parsed.Signed;
            }

            if (parsed.Unsigned.HasValue)
            {
                result = (Convert.ToUInt64(value, CultureInfo.InvariantCulture) & (ulong)parsed.Unsigned)
                       == (ulong)parsed.Unsigned;
            }

            return result;
        }

        /// <summary>
        /// Includes an enumerated type and returns the new value
        /// </summary>
        /// <typeparam name="T">The type of the value(s) being appended.</typeparam>
        /// <param name="value">The <see cref="Enum" /> to add values to.</param>
        /// <param name="append">The value(s) to append.</param>
        /// <returns>A new enumeration of the specified type with the
        /// value(s) in the parameter append included.</returns>
        public static T Include<T>(this Enum value, T append)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            var type = value.GetType();

            var appendType = typeof(T);

            if (appendType != type)
                throw new ArgumentException("The enums have to be of the same type", "append");

            // determine the values
            object result = value;
            var parsed = new Value(append, type);
            if (parsed.Signed.HasValue)
                result = Convert.ToInt64(value, CultureInfo.InvariantCulture) | (long)parsed.Signed;
            else if (parsed.Unsigned.HasValue)
                result = Convert.ToUInt64(value, CultureInfo.InvariantCulture) | (ulong)parsed.Unsigned;

            // return the final value
            return (T)Enum.Parse(type, result.ToString());
        }

        /// <summary>
        /// Checks if an enumerated type is missing a value
        /// </summary>
        /// <typeparam name="T">The type of the <c>value</c> parameter.</typeparam>
        /// <param name="obj">The enumeration value to check.</param>
        /// <param name="value">The value(s) to test for.</param>
        /// <returns>True if <c>obj</c> is missing all values
        /// in <c>value</c>, false otherwise.</returns>
        public static bool Missing<T>(this Enum obj, T value)
        {
            return !Has(obj, value);
        }

        /// <summary>
        /// Removes an enumerated type and returns the new value
        /// </summary>
        /// <typeparam name="T">The type of the value(s) being removed.</typeparam>
        /// <param name="value">The <see cref="Enum" /> to remove values from.</param>
        /// <param name="toRemove">The value(s) to remove from the enumeration.</param>
        /// <returns>A new enumeration of the specified type with the value(s)
        /// supplied in <c>remove</c> removed.</returns>
        public static T Remove<T>(this Enum value, T toRemove)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            var type = value.GetType();

            var removeType = typeof(T);

            if (removeType != type)
                throw new ArgumentException("The enums have to be of the same type", "toRemove");

            // determine the values
            object result = value;
            var parsed = new Value(toRemove, type);
            if (parsed.Signed.HasValue)
                result = Convert.ToInt64(value, CultureInfo.InvariantCulture) & ~(long)parsed.Signed;
            else if (parsed.Unsigned.HasValue)
                result = Convert.ToUInt64(value, CultureInfo.InvariantCulture) & ~(ulong)parsed.Unsigned;

            // return the final value
            return (T)Enum.Parse(type, result.ToString());
        }

        /// <summary>
        /// Class to simplify narrowing values between
        /// a unsigned long and long since either value should
        /// cover any lesser value.
        /// </summary>
        private struct Value
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
            /// Initializes a new instance of the <see cref="Value" /> struct.
            /// </summary>
            /// <param name="value">The value.</param>
            /// <param name="type">The type of <c>value</c>.</param>
            public Value(object value, Type type)
            {
                // check for the enumerated value
                var compare = Enum.GetUnderlyingType(type);

                // if this is an unsigned long then the only
                // value that can hold it would be a ulong
                if (compare == CachedUInt64)
                {
                    Signed = null;
                    Unsigned = Convert.ToUInt64(value, CultureInfo.InvariantCulture);
                }
                else
                {
                    // otherwise, a long should cover anything else
                    Signed = Convert.ToInt64(value, CultureInfo.InvariantCulture);
                    Unsigned = null;
                }
            }
        }
    }
}
