using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLib
{
    public static class SafeConvert
    {
        public static int ToInt(object value)
        {
            int returnValue;
            int.TryParse(Convert.ToString(value), out returnValue);
            return returnValue;
        }

        public static decimal ToDecimal(object value)
        {
            decimal returnValue;
            decimal.TryParse(Convert.ToString(value), System.Globalization.NumberStyles.Currency, null, out returnValue);
            return returnValue;
        }

        public static decimal ToDecimal(object value, System.Globalization.NumberStyles style)
        {
            decimal returnValue;
            decimal.TryParse(Convert.ToString(value), style, null, out returnValue);
            return returnValue;
        }

        public static double ToDouble(object value)
        {
            double returnValue;
            double.TryParse(Convert.ToString(value), out returnValue);
            return returnValue;
        }

        public static DateTime ToDateTime(object value)
        {
            DateTime returnValue;
            DateTime.TryParse(Convert.ToString(value), out returnValue);
            return returnValue;
        }

        public static DateTime? ToDateTime(string value, string format)
        {
            try
            {
                return DateTime.ParseExact(value, format, null);
            }
            catch { return null; }
        }

        public static bool ToBoolean(object value)
        {
            bool returnValue;
            bool.TryParse(Convert.ToString(value), out returnValue);
            return returnValue;
        }

        public static Guid ToGuid(object value)
        {
            try
            {
                return new Guid(Convert.ToString(value));
            }
            catch
            {
                return Guid.Empty;
            }
        }

        public static T To<T>(object value) where T : struct
        {
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }

        public static Nullable<T> ToNullable<T>(object value) where T : struct
        {
            return ToNullable<T>(value, false);
        }

        public static Nullable<T> ToNullable<T>(object value, bool treatDefaultAsNull) where T : struct
        {
            try
            {
                Nullable<T> result = (T)Convert.ChangeType(value, typeof(T));
                return (treatDefaultAsNull && object.Equals(result, default(T))) ? null : result;
            }
            catch
            {
                return null;
            }
        }

        public static string ToCurrency(object value, bool showSymbol)
        {
            return string.Format(showSymbol ? "{0:C}" : "{0:N2}", value == null ? 0 : value);
        }

        public static string ToCurrency(object value)
        {
            return ToCurrency(value, true);
        }

        public static string ToPercentage(object value, bool showSymbol)
        {
            return string.Format(showSymbol ? "{0:P3}" : "{0:N3}", value == null ? 0 : value);
        }

        public static string ToPercentage(object value)
        {
            return ToPercentage(value, true);
        }

        public static string ToOrdinal(int number)
        {
            if (number <= 0)
                return null;

            string ordinal = number.ToString();
            if (number.ToString().EndsWith("1"))
                ordinal += "st";
            else if (number.ToString().EndsWith("2"))
                ordinal += "nd";
            else if (number.ToString().EndsWith("3"))
                ordinal += "rd";
            else
                ordinal += "th";
            return ordinal;
        }

        public static bool IsNullOrDefault<T>(this T argument)
        {
            // deal with normal scenarios
            if (argument == null) return true;
            if (object.Equals(argument, default(T))) return true;

            // deal with non-null nullables
            Type methodType = typeof(T);
            if (Nullable.GetUnderlyingType(methodType) != null) return false;

            // deal with boxed value types
            Type argumentType = argument.GetType();
            if (argumentType.IsValueType && argumentType != methodType)
            {
                object obj = Activator.CreateInstance(argument.GetType());
                return obj.Equals(argument);
            }

            return false;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || enumerable.Count() == 0;
        }

        public static string ToStringOrEmpty(this object obj)
        {
            return (obj ?? string.Empty).ToString();
        }
    }
}
