using CSVReader.Exceptions;
using CSVReader.Extensions;
using CSVReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSVReader.Deserializers
{
    internal class ValueDeserializer
        : BaseDeserializer
    {
        #region Private Fields

        private readonly IEnumerable<ChildDefinition> childDefinitions;
        private readonly Func<object> contentGetter;
        private readonly int fromIndex;
        private readonly IDictionary<int, Action<string>> valuesSetters;

        private object content;
        private BaseDeserializer currentDeserializer;

        #endregion Private Fields

        #region Public Constructors

        public ValueDeserializer(Type type)
        {
            HeaderRegex = type.GetHeaderRegex();
            fromIndex = HeaderRegex == default ? 0 : 1;

            contentGetter = GetContentGetter(type);
            valuesSetters = GetValueSetters(type);
            childDefinitions = GetChildDefinitions(type).ToArray();

            CheckChilds();
        }

        #endregion Public Constructors

        #region Public Methods

        public override object Get()
        {
            foreach (var childDefinition in childDefinitions)
            {
                if (content != default)
                    childDefinition.Setter.Invoke();
            }

            var result = content;
            content = default;

            return result;
        }

        public override void Set(IEnumerable<string> values)
        {
            if (values?.Any() ?? false)
            {
                var header = values.First();

                if ((HeaderRegex?.IsMatch(header) ?? false)
                    || !childDefinitions.Any())
                {
                    if (content != default)
                        throw new PropertyAlreadySetException();

                    content = contentGetter.Invoke();

                    for (var index = fromIndex; index < values.Count(); index++)
                    {
                        if (valuesSetters.ContainsKey(index))
                        {
                            valuesSetters[index]?.Invoke(values.ElementAt(index));
                        }
                    }
                }
                else
                {
                    if (!(currentDeserializer?.HeaderRegex?.IsMatch(header) ?? false))
                    {
                        content = contentGetter.Invoke();

                        currentDeserializer = childDefinitions
                            .FirstOrDefault(c => c.HeaderRegex?.IsMatch(header) ?? true)?.Deserializer;
                    }

                    currentDeserializer?.Set(values);
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void CheckChilds()
        {
            var sameRecordHeaders = childDefinitions
                .GroupBy(r => r.HeaderRegex)
                .Where(g => g.Count() > 1).ToArray();

            if (sameRecordHeaders.Any())
                throw new SameRecordHeaderException(
                    sameRecordHeaders.First().First().HeaderRegex?.ToString());
        }

        private IEnumerable<ChildDefinition> GetChildDefinitions(Type type)
        {
            var properties = type.GetChildProperties().ToArray();

            foreach (var property in properties)
            {
                var deserializer = property.PropertyType.IsClassEnumerable()
                    ? new EnumerableDeserializer(property.PropertyType) as BaseDeserializer
                    : new ValueDeserializer(property.PropertyType) as BaseDeserializer;

                var result = new ChildDefinition
                {
                    Deserializer = deserializer,
                    HeaderRegex = deserializer.HeaderRegex,
                    Setter = () => property.SetValue(
                        obj: content,
                        value: deserializer.Get()),
                };

                yield return result;
            }
        }

        private Func<object> GetContentGetter(Type type)
        {
            return () => Activator.CreateInstance(type);
        }

        private IDictionary<int, Action<string>> GetValueSetters(Type type)
        {
            var fieldDescriptions = type.GetFieldDefinitions()
                .OrderBy(d => d.Index).ToArray();

            var result = new Dictionary<int, Action<string>>();

            foreach (var fieldDescription in fieldDescriptions)
            {
                var isList = fieldDescription.Type.IsListType();
                var length = isList ? fieldDescription.Length : 1;

                for (var position = 0; position < length; position++)
                {
                    var setter = default(Action<string>);

                    if (isList)
                    {
                        var listType = fieldDescription.Type.GetContentType().GetListType();

                        if (fieldDescription.Type.GetContentType() == typeof(string))
                        {
                            setter = (text) => fieldDescription.Property.AddText(
                                listType: listType,
                                content: content,
                                text: text);
                        }
                        else
                        {
                            setter = (text) => fieldDescription.Property.AddValue(
                                listType: listType,
                                content: content,
                                text: text);
                        }
                    }
                    else if (fieldDescription.Type == typeof(DateTime) || fieldDescription.Type == typeof(DateTime?))
                    {
                        setter = (text) => fieldDescription.Property.SetDateTime(
                            content: content,
                            text: text,
                            format: fieldDescription.Format);
                    }
                    else if (fieldDescription.Type == typeof(TimeSpan) || fieldDescription.Type == typeof(TimeSpan?))
                    {
                        setter = (text) => fieldDescription.Property.SetTimeSpan(
                            content: content,
                            text: text,
                            format: fieldDescription.Format);
                    }
                    else if (fieldDescription.Type == typeof(string))
                    {
                        setter = (text) => fieldDescription.Property.SetText(
                            content: content,
                            text: text);
                    }
                    else
                    {
                        setter = (text) => fieldDescription.Property.SetValue(
                            content: content,
                            text: text);
                    }

                    if (setter != default)
                    {
                        result.Add(
                            key: fieldDescription.Index + position,
                            value: setter);
                    }
                }
            }

            return result;
        }

        #endregion Private Methods
    }
}