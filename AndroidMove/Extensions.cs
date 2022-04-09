using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AndroidMove {
    public static class Extensions {
        public static int? ToIntN(this object value) {
            if (value == null || value == DBNull.Value) {
                return null;
            }
            int ret;
            return int.TryParse(value.ToString(), out ret) ? ret : (int?)null;
        }

        internal static int ToInt32(this object value, int defaultValue) {
            return value.ToIntN() ?? defaultValue;
        }

        internal static DateTime? ToDateTime(this object value) {
            if (value == null || value == DBNull.Value) {
                return null;
            }
            if (value is DateTime) {
                return (DateTime)value;
            }
            if (value is DateTime?) {
                return (DateTime?)value;
            }
            return null;
        }

        internal static DateTime? ToDateTime(this object value, string dateFormat) {
            if (value == null || value == DBNull.Value) {
                return null;
            }
            DateTime d;
            return DateTime.TryParseExact(value.ToString(), dateFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out d) ? d : (DateTime?)null;
        }

        internal static decimal? ToDecimalN(this object value) {
            if (value == null || value == DBNull.Value) {
                return null;
            }
            decimal ret;
            return decimal.TryParse(value.ToString(), out ret) ? ret : (decimal?)null;
        }

        internal static decimal ToDecimal(this object value, decimal defaultValue) {
            return value.ToDecimalN() ?? defaultValue;
        }
        internal static float? ToFloatN(this object value) {
            if (value == null || value == DBNull.Value) {
                return null;
            }
            float ret;
            return float.TryParse(value.ToString(), out ret) ? ret : (float?)null;
        }

        internal static float ToFloat(this object value, float defaultValue) {
            return value.ToFloatN() ?? defaultValue;
        }

        internal static double? ToDoubleN(this object value) {
            if (value == null || value == DBNull.Value) {
                return null;
            }
            double ret;
            return double.TryParse(value.ToString(), out ret) ? ret : (double?)null;
        }

        internal static double ToDouble(this object value, double defaultValue) {
            return value.ToDoubleN() ?? defaultValue;
        }
        internal static long? ToLongN(this object value) {
            if (value == null || value == DBNull.Value) {
                return null;
            }
            long ret;
            return long.TryParse(value.ToString(), out ret) ? ret : (long?)null;
        }

        internal static long ToLong(this object value, long defaultValue) {
            return value.ToLongN() ?? defaultValue;
        }
    }
}