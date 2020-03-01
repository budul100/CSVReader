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

        public static Type GetContentType(this Type type)
        {
            return type.GetGenericArguments().FirstOrDefault()
                ?? type.GetElementType();
        }

        public static IEnumerable<FieldDescription> GetFieldDescriptions(this Type type)
        {
            var properties = type.GetProperties()
                .Where(p => !(p.PropertyType.IsClassType() || p.PropertyType.IsEnumerableType())).ToArray();

            foreach (var property in properties)
            {
                var index = property.GetAttribute<ImportField>()?.Index;

                if (index.HasValue)
                {
                    var length = property.GetAttribute<ImportField>()?.Length ?? 1;

                    var result = new FieldDescription
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

        public static bool IsEnumerableType(this Type type)
        {
            if (type == null || type == typeof(string))
            {
                return false;
            }

            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        #endregion Public Methods
    }
}