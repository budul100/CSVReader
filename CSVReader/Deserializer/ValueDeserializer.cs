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

        private readonly IEnumerable<ChildDeserializer> childDeserializers;
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
            fromIndex = HeaderRegex == null ? 0 : 1;

            contentGetter = GetContentGetter(type);
            valuesSetters = GetValueSetters(type);
            childDeserializers = GetChilds(type).ToArray();

            CheckChilds();
        }

        #endregion Public Constructors

        #region Public Methods

        public override object Get()
        {
            foreach (var childDeserializer in childDeserializers)
            {
                childDeserializer.Setter.Invoke();
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

                if (HeaderRegex?.IsMatch(header) ?? true)
                {
                    if (content != null)
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
                    if (!(currentDeserializer?.HeaderRegex.IsMatch(header) ?? false))
                    {
                        currentDeserializer = childDeserializers
                            .SingleOrDefault(c => c.HeaderRegex.IsMatch(header))?.Deserializer;
                    }

                    currentDeserializer?.Set(values);
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void CheckChilds()
        {
            var sameRecordHeaders = childDeserializers
                .GroupBy(r => r.HeaderRegex)
                .Where(g => g.Count() > 1).ToArray();

            if (sameRecordHeaders.Any())
                throw new SameRecordHeaderException(
                    sameRecordHeaders.First().First().HeaderRegex?.ToString());
        }

        private IEnumerable<ChildDeserializer> GetChilds(Type type)
        {
            var properties = type.GetProperties()
                .Where(p => p.PropertyType.IsClassType()
                    || p.PropertyType.IsEnumerableType()).ToArray();

            foreach (var property in properties)
            {
                var isEnumerable = property.PropertyType.IsEnumerableType();

                var deserializer = isEnumerable
                    ? new EnumerableDeserializer(property.PropertyType) as BaseDeserializer
                    : new ValueDeserializer(property.PropertyType) as BaseDeserializer;

                if (deserializer.HeaderRegex != default)
                {
                    var result = new ChildDeserializer
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
        }

        private Func<object> GetContentGetter(Type type)
        {
            return () => Activator.CreateInstance(type);
        }

        private IDictionary<int, Action<string>> GetValueSetters(Type type)
        {
            var fieldDescriptions = type.GetFieldDescriptions()
                .OrderBy(d => d.Index).ToArray();

            var result = new Dictionary<int, Action<string>>();

            foreach (var fieldDescription in fieldDescriptions)
            {
                var isList = fieldDescription.Type.IsGenericType
                    && fieldDescription.Type.GetGenericTypeDefinition() == typeof(List<>);

                var length = isList ? fieldDescription.Length : 1;

                for (var position = 0; position < length; position++)
                {
                    var setter = default(Action<string>);

                    if (fieldDescription.Type == typeof(DateTime) || fieldDescription.Type == typeof(DateTime?))
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
                    else if (fieldDescription.Type == typeof(List<string>))
                    {
                        setter = (text) => fieldDescription.Property.AddText(
                            listType: fieldDescription.Type,
                            content: content,
                            text: text);
                    }
                    else if (isList)
                    {
                        setter = (text) => fieldDescription.Property.AddValue(
                            listType: fieldDescription.Type,
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