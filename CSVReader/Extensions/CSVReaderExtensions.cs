using CSVReader.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TB.ComponentModel;

namespace CSVReader.Extensions
{
    internal static class CSVReaderExtensions
    {
        #region Public Methods

        public static T GetAttribute<T>(this Type type)
            where T : class
        {
            return type.GetCustomAttribute(typeof(T)) as T;
        }

        public static T GetAttribute<T>(this PropertyInfo property)
            where T : class
        {
            return property.GetCustomAttribute(typeof(T)) as T;
        }

        public static Type GetContentType(this Type type)
        {
            return type.GetGenericArguments().FirstOrDefault()
                ?? type.GetElementType();
        }

        public static Regex GetHeaderRegex(this Type type)
        {
            var headerRegex = type.GetAttribute<ImportHeader>()?.HeaderRegex;

            return string.IsNullOrWhiteSpace(headerRegex)
                ? default
                : new Regex($"^{headerRegex}$");
        }

        public static Type GetListType(this Type type)
        {
            return typeof(List<>).MakeGenericType(type);
        }

        public static bool IsClassProperty(this PropertyInfo property)
        {
            return property != null && property.PropertyType.IsClassType();
        }

        public static bool IsClassType(this Type type)
        {
            if (type == null || type == typeof(string) || type.IsEnumerableType())
            {
                return false;
            }

            return type.IsClass;
        }

        public static bool IsEnumerableProperty(this PropertyInfo property)
        {
            return property != null && property.PropertyType.IsEnumerableType();
        }

        public static bool IsEnumerableType(this Type type)
        {
            if (type == null || type == typeof(string))
            {
                return false;
            }

            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        public static void SetDateTime(this PropertyInfo property, object content, string text, string format)
        {
            var value = text.Trim();

            if (!string.IsNullOrWhiteSpace(format))
            {
                var converted = text.GetDateTime(format);

                property.SetValue(
                    obj: content,
                    value: converted);
            }
            else if (DateTime.TryParse(
                s: value,
                provider: CultureInfo.InvariantCulture,
                styles: DateTimeStyles.None,
                result: out DateTime converted))
            {
                property.SetValue(
                    obj: content,
                    value: converted);
            }
        }

        public static void SetText(this PropertyInfo property, object content, string text)
        {
            var value = text.Trim();

            if (!string.IsNullOrWhiteSpace(value))
            {
                property.SetValue(
                    obj: content,
                    value: value);
            }
        }

        public static void SetTimeSpan(this PropertyInfo property, object content, string text, string format)
        {
            var value = text.Trim();

            if (!string.IsNullOrWhiteSpace(format))
            {
                var converted = text.GetTimeSpan(format);

                property.SetValue(
                    obj: content,
                    value: converted);
            }
            else if (TimeSpan.TryParse(
                input: value,
                formatProvider: CultureInfo.InvariantCulture,
                result: out TimeSpan converted))
            {
                property.SetValue(
                    obj: content,
                    value: converted);
            }
        }

        public static void SetValue(this PropertyInfo property, object content, string text)
        {
            var value = text.Trim();

            if (!string.IsNullOrWhiteSpace(value)
                && value.IsConvertibleTo(property.PropertyType))
            {
                property.SetValue(
                    obj: content,
                    value: value.To(property.PropertyType));
            }
        }

        #endregion Public Methods

        #region Private Methods

        private static DateTime? GetDateTime(this string value, string format, int offset = 0) // Offset eliminates need for substring
        {
            if (string.IsNullOrWhiteSpace(value))
                return default;

            var year = 0;
            var month = 0;
            var day = 0;
            var hour = 0;
            var minute = 0;
            var second = 0;
            var milliSecond = 0;
            var hourOffset = 0;

            if (value.Length + offset < format.Length)
                throw new Exception($"Date string \"{value.Substring(offset)}\" too short for format \"{format}\"");

            for (var i = 0; i < format.Length; i++)
            {
                var c = value[offset + i];

                switch (format[i])
                {
                    case 'y':
                        year = year * 10 + (c - '0');
                        break;

                    case 'M':
                        month = month * 10 + (c - '0');
                        break;

                    case 'd':
                        day = day * 10 + (c - '0');
                        break;

                    case 'T':
                        if (c == 'p' || c == 'P')
                            hourOffset = 12;
                        break;

                    case 'h':
                        hour = hour * 10 + (c - '0');
                        if (hour == 12) hour = 0;
                        break;

                    case 'H':
                        hour = hour * 10 + (c - '0');
                        hourOffset = 0;
                        break;

                    case 'm':
                        minute = minute * 10 + (c - '0');
                        break;

                    case 's':
                        second = second * 10 + (c - '0');
                        break;

                    case 'f':
                        milliSecond = milliSecond * 10 + (c - '0');
                        break;
                }
            }

            year += year > 30 ? 1900 : 2000;

            try
            {
                return new DateTime(
                    year: year,
                    month: month,
                    day: day,
                    hour: hour + hourOffset,
                    minute: minute,
                    second: second,
                    millisecond: milliSecond);
            }
            catch (Exception)
            {
                throw new Exception($"Error in date / time: {year}-{month}-{day} {hour}:{minute}:{second}.{milliSecond} " +
                    $"- {format} {value.Substring(offset, format.Length)}");
            }
        }

        private static TimeSpan? GetTimeSpan(this string value, string format, int offset = 0) // Offset eliminates need for substring
        {
            if (string.IsNullOrWhiteSpace(value))
                return default;

            var days = 0;
            var hours = 0;
            var minutes = 0;
            var seconds = 0;
            var milliSeconds = 0;

            if (value.Length + offset < format.Length)
                throw new Exception($"Date string \"{value.Substring(offset)}\" too short for format \"{format}\"");

            for (var i = 0; i < format.Length; i++)
            {
                var c = value[offset + i];

                switch (format[i])
                {
                    case 'd':
                        days = days * 10 + (c - '0');
                        break;

                    case 'h':
                        hours = hours * 10 + (c - '0');
                        if (hours == 12) hours = 0;
                        break;

                    case 'm':
                        minutes = minutes * 10 + (c - '0');
                        break;

                    case 's':
                        seconds = seconds * 10 + (c - '0');
                        break;

                    case 'f':
                        milliSeconds = milliSeconds * 10 + (c - '0');
                        break;
                }
            }

            try
            {
                return new TimeSpan(
                    days: days,
                    hours: hours,
                    minutes: minutes,
                    seconds: seconds,
                    milliseconds: milliSeconds);
            }
            catch (Exception)
            {
                throw new Exception($"Error in time: {days} {hours}:{minutes}:{seconds}.{milliSeconds} " +
                    $"- {format} {value.Substring(offset, format.Length)}");
            }
        }

        #endregion Private Methods
    }
}