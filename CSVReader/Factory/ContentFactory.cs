using CSVReader.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using TB.ComponentModel;

namespace CSVReader.Factory
{
    internal class ContentFactory
    {
        #region Private Fields

        private readonly Type type;

        #endregion Private Fields

        #region Public Constructors

        public ContentFactory(Type type)
        {
            var fieldDescriptions = type.GetFieldDefinitions()
                .OrderBy(d => d.Index).ToArray();

            Setters = new Dictionary<int, Action<object, string>>();

            foreach (var fieldDescription in fieldDescriptions)
            {
                var isList = fieldDescription.Type.IsListType();
                var length = isList ? fieldDescription.Length : 1;

                for (var position = 0; position < length; position++)
                {
                    var setter = default(Action<object, string>);

                    if (isList)
                    {
                        var listType = fieldDescription.Type.GetContentType().GetListType();

                        if (fieldDescription.Type.GetContentType() == typeof(string))
                        {
                            setter = (content, text) => AddText(
                                property: fieldDescription.Property,
                                listType: listType,
                                content: content,
                                text: text);
                        }
                        else
                        {
                            setter = (content, text) => AddValue(
                                property: fieldDescription.Property,
                                listType: listType,
                                content: content,
                                text: text);
                        }
                    }
                    else if (fieldDescription.Type == typeof(DateTime) || fieldDescription.Type == typeof(DateTime?))
                    {
                        setter = (content, text) => SetDateTime(
                            property: fieldDescription.Property,
                            content: content,
                            text: text,
                            format: fieldDescription.Format);
                    }
                    else if (fieldDescription.Type == typeof(TimeSpan) || fieldDescription.Type == typeof(TimeSpan?))
                    {
                        setter = (content, text) => SetTimeSpan(
                            property: fieldDescription.Property,
                            content: content,
                            text: text,
                            format: fieldDescription.Format);
                    }
                    else if (fieldDescription.Type == typeof(string))
                    {
                        setter = (content, text) => SetText(
                            property: fieldDescription.Property,
                            content: content,
                            text: text);
                    }
                    else
                    {
                        setter = (content, text) => SetValue(
                            property: fieldDescription.Property,
                            content: content,
                            text: text);
                    }

                    if (setter != default)
                    {
                        Setters.Add(
                            key: fieldDescription.Index + position,
                            value: setter);
                    }
                }
            }

            this.type = type;
        }

        #endregion Public Constructors

        #region Public Properties

        public Func<object> Getter => () => Activator.CreateInstance(type);

        public IDictionary<int, Action<object, string>> Setters { get; }

        #endregion Public Properties

        #region Private Methods

        private static IList GetList(PropertyInfo property, Type listType, object content)
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

        private void AddText(PropertyInfo property, Type listType, object content, string text)
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

        private void AddValue(PropertyInfo property, Type listType, object content, string text)
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

        private void SetDateTime(PropertyInfo property, object content, string text, string format)
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

        private void SetText(PropertyInfo property, object content, string text)
        {
            var value = text.Trim();

            if (!string.IsNullOrWhiteSpace(value))
            {
                property.SetValue(
                    obj: content,
                    value: value);
            }
        }

        private void SetTimeSpan(PropertyInfo property, object content, string text, string format)
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

        private void SetValue(PropertyInfo property, object content, string text)
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

        #endregion Private Methods
    }
}