using CSVReader.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSVReader.Extensions
{
    internal static class TypeExtensions
    {
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
            return type.GetGenericArguments().FirstOrDefault()
                ?? type.GetElementType()
                ?? type;
        }

        public static IEnumerable<PropertyInfo> GetFieldProperties(this Type type)
        {
            var result = type.GetProperties()
                .Where(p => p.GetAttribute<FieldAttribute>() != default)
                .Where(p => !p.PropertyType.GetContentType().IsClassType())
                .OrderBy(p => p.GetAttribute<FieldAttribute>().Index).ToArray();

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