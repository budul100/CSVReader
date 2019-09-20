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
            var headerRegex = type.GetAttribute<ImportRecord>()?.HeaderRegex;

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

        public static void SetDateTime(this PropertyInfo property, object content, string text)
        {
            var value = text.Trim();

            if (DateTime.TryParse(
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

        public static void SetTimeSpan(this PropertyInfo property, object content, string text)
        {
            var value = text.Trim();

            if (TimeSpan.TryParse(
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
    }
}