using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using TB.ComponentModel;

namespace CSVReader.Extensions
{
    internal static class PropertyExtensions
    {
        #region Public Methods

        public static void AddText(this PropertyInfo property, Type listType, object content, string text)
        {
            var value = text.Trim();

            if (!string.IsNullOrWhiteSpace(value))
            {
                var list = GetList(
                    property: property,
                    listType: listType,
                    content: content);

                list.Add(value);
            }
        }

        public static void AddValue(this PropertyInfo property, Type listType, object content, string text)
        {
            var value = text.Trim();

            if (!string.IsNullOrWhiteSpace(value)
                && value.IsConvertibleTo(property.PropertyType))
            {
                var list = GetList(
                    property: property,
                    listType: listType,
                    content: content);

                list.Add(value.To(property.PropertyType));
            }
        }

        public static IList GetList(this PropertyInfo property, Type listType, object content)
        {
            var list = property.GetValue(
                obj: content,
                index: default) as IList;

            if (list == default)
            {
                list = Activator.CreateInstance(listType) as IList;

                property.SetValue(
                    obj: content,
                    value: list);
            }

            return list;
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
    }
}