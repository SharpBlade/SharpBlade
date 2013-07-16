// Code for EnumExtensions class written by Hugoware
// http://hugoware.net/blog/enumeration-extensions-2-0
// Modifications done by Sharparam: ReSharper cleanup

using System;

namespace Sharparam.SharpBlade.Extensions
{
    /// <summary>
    /// Extension methods to make working with Enum values easier
    /// </summary>
    public static class EnumerationExtensions
    {
        #region Extension Methods

        /// <summary>
        /// Includes an enumerated type and returns the new value
        /// </summary>
        public static T Include<T>(this Enum value, T append)
        {
            var type = value.GetType();

            //determine the values
            object result = value;
            var parsed = new _Value(append, type);
            if (parsed.Signed.HasValue) //if (parsed.Signed is long)
            {
                result = Convert.ToInt64(value) | (long)parsed.Signed;
            }
            else if (parsed.Unsigned.HasValue) //else if (parsed.Unsigned is ulong)
            {
                result = Convert.ToUInt64(value) | (ulong)parsed.Unsigned;
            }

            //return the final value
            return (T)Enum.Parse(type, result.ToString());
        }

        /// <summary>
        /// Removes an enumerated type and returns the new value
        /// </summary>
        public static T Remove<T>(this Enum value, T remove)
        {
            Type type = value.GetType();

            //determine the values
            object result = value;
            var parsed = new _Value(remove, type);
            if (parsed.Signed.HasValue) //if (parsed.Signed is long)
            {
                result = Convert.ToInt64(value) & ~(long)parsed.Signed;
            }
            else if (parsed.Unsigned.HasValue) //else if (parsed.Unsigned is ulong)
            {
                result = Convert.ToUInt64(value) & ~(ulong)parsed.Unsigned;
            }

            //return the final value
            return (T)Enum.Parse(type, result.ToString());
        }

        /// <summary>
        /// Checks if an enumerated type contains a value
        /// </summary>
        public static bool Has<T>(this Enum value, T check)
        {
            Type type = value.GetType();

            //determine the values
            var parsed = new _Value(check, type);
            if (parsed.Signed.HasValue) //if (parsed.Signed is long)
            {
                return (Convert.ToInt64(value) & (long)parsed.Signed) == (long)parsed.Signed;
            }
            if (parsed.Unsigned.HasValue) //if (parsed.Unsigned is ulong)
            {
                return (Convert.ToUInt64(value) & (ulong)parsed.Unsigned) == (ulong)parsed.Unsigned;
            }
            return false;
        }

        /// <summary>
        /// Checks if an enumerated type is missing a value
        /// </summary>
        public static bool Missing<T>(this Enum obj, T value)
        {
            return !Has(obj, value);
        }

        #endregion

        #region Helper Classes

        //class to simplfy narrowing values between 
        //a ulong and long since either value should
        //cover any lesser value
        private class _Value
        {

            //cached comparisons for tye to use
            private static readonly Type _UInt64 = typeof(ulong);
            private static readonly Type _UInt32 = typeof(long);

            public readonly long? Signed;
            public readonly ulong? Unsigned;

            public _Value(object value, Type type)
            {

                //make sure it is even an enum to work with
                if (!type.IsEnum)
                    throw new ArgumentException("Value provided is not an enumerated type!");

                //then check for the enumerated value
                var compare = Enum.GetUnderlyingType(type);

                //if this is an unsigned long then the only
                //value that can hold it would be a ulong
                if (compare == _UInt32 || compare == _UInt64)
                    Unsigned = Convert.ToUInt64(value);
                else //otherwise, a long should cover anything else
                    Signed = Convert.ToInt64(value);
            }
        }

        #endregion
    }
}
