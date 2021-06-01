using CSVReader.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSVReader.Extensions
{
    internal static class PropertyExtensions
    {
        #region Public Methods

        public static T GetAttribute<T>(this PropertyInfo property)
            where T : class
        {
            return property.GetCustomAttribute(typeof(T)) as T;
        }

        public static IEnumerable<Func<string, string>> GetFixedGetters(this IEnumerable<PropertyInfo> properties)
        {
            if (properties?.Any() ?? false)
            {
                foreach (var property in properties)
                {
                    var attribute = property.GetAttribute<FixedFieldAttribute>();

                    if (attribute != default)
                    {
                        string result(string line) => line.GetFixedText(
                            start: attribute.Start,
                            length: attribute.Length);

                        yield return result;
                    }
                }
            }
        }

        #endregion Public Methods
    }
}