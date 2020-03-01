using CSVReader.Attributes;
using CSVReader.Models;
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
        #region Public Methods

        public static T GetAttribute<T>(this Type type)
            where T : class
        {
            return type.GetCustomAttribute(typeof(T)) as T;
        }

        public static IEnumerable<PropertyInfo> GetChildProperties(this Type type)
        {
            var result = type.GetProperties()
                .Where(p => p.PropertyType.IsClassType()
                    || p.PropertyType.IsClassEnumerable()).ToArray();

            return result;
        }

        public static Type GetContentType(this Type type)
        {
            return type.GetGenericArguments().FirstOrDefault()
                ?? type.GetElementType();
        }

        public static IEnumerable<FieldDefinition> GetFieldDefinitions(this Type type)
        {
            var properties = type.GetProperties()
                .Where(p => p.GetAttribute<ImportField>() != default)
                .Where(p => !p.PropertyType.IsClassType())
                .Where(p => !p.PropertyType.IsClassEnumerable()).ToArray();

            foreach (var property in properties)
            {
                var index = property.GetAttribute<ImportField>()?.Index;

                if (index.HasValue)
                {
                    var length = property.GetAttribute<ImportField>()?.Length ?? 1;

                    var result = new FieldDefinition
                    {
                        Format = property.GetAttribute<ImportField>()?.Format,
                        Index = index.Value,
                        Length = length,
                        Property = property,
                        Type = property.PropertyType,
                    };

                    yield return result;
                };
            };
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

        public static bool IsClassEnumerable(this Type type)
        {
            var result = type.GetInterfaces()
                .Where(t => t.IsGenericType)
                .Where(t => t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .Select(t => t.GetContentType())
                .FirstOrDefault().IsClassType();

            return result;
        }

        public static bool IsClassType(this Type type)
        {
            if (type == default || type == typeof(string) || type.IsEnumerableType())
            {
                return false;
            }

            return type.IsClass;
        }

        public static bool IsEnumerableType(this Type type)
        {
            if (type == null || type == typeof(string))
            {
                return false;
            }

            return typeof(IEnumerable).IsAssignableFrom(type);
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