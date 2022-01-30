using CSVReader.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CSVReader.Extensions
{
    internal static class TypeExtensions
    {
        #region Private Fields

        private const string DefaultDelimiter = ",";

        #endregion Private Fields

        #region Public Methods

        public static IList GetAsList(this Type type)
        {
            var contentType = type.GetContentType();
            var listType = typeof(List<>).MakeGenericType(contentType);

            var result = Activator.CreateInstance(listType);
            return result as IList;
        }

        public static T GetAttribute<T>(this Type type)
            where T : class
        {
            return type.GetCustomAttribute(typeof(T)) as T;
        }

        public static IEnumerable<PropertyInfo> GetChildProperties(this Type type)
        {
            var result = type.GetProperties()
                .Where(p => p.PropertyType.GetContentType().IsClassType())
                .Where(p => p.PropertyType.GetContentType().GetAttribute<TypeAttribute>() != default).ToArray();

            return result;
        }

        public static Type GetContentType(this Type type)
        {
            var result = type.GetGenericArguments().FirstOrDefault()
                ?? type.GetElementType()
                ?? type;

            return result;
        }

        public static IEnumerable<PropertyInfo> GetDelimitedsProperties(this Type type)
        {
            var result = type.GetProperties()
                .Where(p => p.GetAttribute<DelimitedFieldAttribute>() != default)
                .Where(p => !p.PropertyType.GetContentType().IsClassType())
                .OrderBy(p => p.GetAttribute<DelimitedFieldAttribute>().Index).ToArray();

            return result;
        }

        public static IEnumerable<PropertyInfo> GetFixedsProperties(this Type type)
        {
            var result = type.GetProperties()
                .Where(p => p.GetAttribute<FixedFieldAttribute>() != default)
                .Where(p => !p.PropertyType.GetContentType().IsClassType())
                .OrderBy(p => p.GetAttribute<FixedFieldAttribute>().Start).ToArray();

            return result;
        }

        public static IEnumerable<char> GetSeparators(this Type type, string given)
        {
            var delimiters = given
                ?? type.GetAttribute<BaseSetAttribute>()?.Delimiters
                ?? DefaultDelimiter;

            var result = delimiters?.ToCharArray()
                ?? Enumerable.Empty<char>();

            return result;
        }

        public static Regex GetTrimRegex(this Type type)
        {
            var result = default(Regex);

            var setAttribute = type.GetAttribute<BaseSetAttribute>();

            if (setAttribute != default
                && !string.IsNullOrWhiteSpace(setAttribute.TrimPattern))
            {
                result = new Regex(
                    pattern: $"{setAttribute.TrimPattern}",
                    options: RegexOptions.Singleline);
            }

            return result;
        }

        public static bool HasFixedFields(this Type type)
        {
            var fixedSetAttribute = type.GetAttribute<FixedSetAttribute>();

            return fixedSetAttribute != default;
        }

        public static bool IsClassType(this Type type)
        {
            if (type == default || type == typeof(string) || type.IsEnumerableType())
            {
                return false;
            }

            return type.IsClass;
        }

        public static bool IsEnumerableClass(this Type type)
        {
            var result = type.IsEnumerableType()
                && type.GetContentType().IsClassType();

            return result;
        }

        public static bool IsEnumerableType(this Type type)
        {
            if (type == null || type == typeof(string))
            {
                return false;
            }

            var result = typeof(IEnumerable).IsAssignableFrom(type);

            return result;
        }

        public static bool IsListType(this Type type)
        {
            var result = type.IsGenericType
                && type.GetGenericTypeDefinition() == typeof(IList<>);

            return result;
        }

        #endregion Public Methods
    }
}