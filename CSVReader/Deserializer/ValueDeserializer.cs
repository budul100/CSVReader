using CSVReader.Attributes;
using CSVReader.Exceptions;
using CSVReader.Extensions;
using CSVReader.Internals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSVReader.Deserializers
{
    internal class ValueDeserializer : IDeserializer
    {
        #region Private Fields

        private readonly IEnumerable<ChildDeserializer> childs;
        private readonly Type type;
        private readonly Action<string>[] valueSetters;

        private ChildDeserializer currentChild;

        private int fromIndex;

        #endregion Private Fields

        #region Public Constructors

        public ValueDeserializer(Type type)
        {
            this.type = type;
            Header = type.GetAttribute<ImportRecord>().Header;
            fromIndex = string.IsNullOrWhiteSpace(Header) ? 0 : 1;

            valueSetters = GetValueSetters().ToArray();
            childs = GetChilds().ToArray();

            CheckChilds();
            Initialize();
        }

        #endregion Public Constructors

        #region Public Properties

        public object Content { get; private set; }
        public string Header { get; }

        #endregion Public Properties

        #region Public Methods

        public void Initialize()
        {
            currentChild = null;

            foreach (var child in childs)
            {
                child.Reader.Initialize();
                child.IsAlreadySet = false;
            }
        }

        public bool Set(string[] values)
        {
            var success = false;

            if (values?.Any() ?? false)
            {
                var header = values[0];

                if (string.IsNullOrWhiteSpace(Header) || header == Header)
                {
                    if (values.Count() > valueSetters.Count())
                        throw new TooManyValuesException();

                    Content = Activator.CreateInstance(type);

                    for (var index = fromIndex; index < values.Count(); index++)
                    {
                        valueSetters[index].Invoke(values[index]);
                    }

                    success = true;
                }
                else
                {
                    if (currentChild != null && !childs.Any(c => c.Reader.Header == header))
                    {
                        success = currentChild.Reader.Set(values);
                    }

                    if (!success)
                    {
                        currentChild = childs
                            .SingleOrDefault(c => c.Reader.Header == values[0]);

                        if (currentChild != null)
                        {
                            if (currentChild.IsAlreadySet && !currentChild.IsEnumerable)
                                throw new PropertyAlreadySetException();

                            success = currentChild.Reader.Set(values);

                            if (success)
                            {
                                if (!currentChild.IsAlreadySet) currentChild.ValueSetter();
                                currentChild.IsAlreadySet = true;
                            }
                        }
                    }
                }
            }

            return success;
        }

        #endregion Public Methods

        #region Private Methods

        private void CheckChilds()
        {
            var sameRecordHeaders = childs
                .GroupBy(r => r.Reader.Header)
                .Where(g => g.Count() > 1).ToArray();

            if (sameRecordHeaders.Any())
                throw new SameRecordHeaderException(sameRecordHeaders.First().First().Reader.Header);
        }

        private IEnumerable<ChildDeserializer> GetChilds()
        {
            var classProperties = type.GetProperties()
                .Where(p => p.PropertyType.IsClassType()).ToArray();

            foreach (var property in classProperties)
            {
                var reader = new ValueDeserializer(property.PropertyType);

                yield return new ChildDeserializer
                {
                    IsEnumerable = false,
                    Reader = reader,
                    ValueSetter = () => property.SetValue(
                        obj: Content,
                        value: reader.Content),
                };
            }

            var enumerableProperties = type.GetProperties()
                .Where(p => p.PropertyType.IsEnumerableType()).ToArray();

            foreach (var property in enumerableProperties)
            {
                var reader = new EnumerableDeserializer(property.PropertyType.GetGenericArguments()[0]);

                yield return new ChildDeserializer
                {
                    IsEnumerable = true,
                    Reader = reader,
                    ValueSetter = () => property.SetValue(
                        obj: Content,
                        value: reader.Content)
                };
            }
        }

        private IEnumerable<Action<string>> GetValueSetters()
        {
            var properties = type.GetProperties()
                .Where(p => !(p.PropertyType.IsClassType() || p.PropertyType.IsEnumerableType()))
                .Select(p => new
                {
                    Property = p,
                    p.GetAttribute<ImportField>()?.Index,
                    p.PropertyType,
                })
                .Where(p => p.Index.HasValue)
                .OrderBy(p => p.Index.Value).ToArray();

            if (!string.IsNullOrWhiteSpace(Header))
            {
                yield return null;
            }

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(string))
                {
                    yield return (text) => property.Property.SetText(
                        content: Content,
                        text: text);
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    yield return (text) => property.Property.SetDateTime(
                        content: Content,
                        text: text);
                }
                else if (property.PropertyType == typeof(TimeSpan))
                {
                    yield return (text) => property.Property.SetTimeSpan(
                        content: Content,
                        text: text);
                }
                else
                {
                    yield return (text) => property.Property.SetValue(
                        content: Content,
                        text: text);
                }
            }
        }

        #endregion Private Methods
    }
}