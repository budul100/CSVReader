using CSVReader.Attributes;
using CSVReader.Exceptions;
using CSVReader.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CSVReader.Deserializers
{
    internal class ValueDeserializer : IDeserializer
    {
        #region Private Fields

        private readonly IEnumerable<Tuple<IDeserializer, Action>> childs;
        private readonly int fromIndex;
        private readonly Func<object> initializeGetter;
        private readonly IEnumerable<Tuple<int, Action<string>>> valuesSetters;

        private object content;
        private IDeserializer currentChild;

        #endregion Private Fields

        #region Public Constructors

        public ValueDeserializer(Type type)
        {
            HeaderRegex = type.GetHeaderRegex();
            fromIndex = HeaderRegex == null ? 0 : 1;

            initializeGetter = GetInitializeGetter(type);
            valuesSetters = GetValueSetters(type).ToArray();
            childs = GetChilds(type).ToArray();

            CheckChilds();
        }

        #endregion Public Constructors

        #region Public Properties

        public Regex HeaderRegex { get; }

        #endregion Public Properties

        #region Public Methods

        public object Get()
        {
            foreach (var child in childs)
            {
                child.Item2.Invoke();
            }

            var result = content;
            content = null;
            return result;
        }

        public void Set(IEnumerable<string> values)
        {
            if (values?.Any() ?? false)
            {
                var header = values.First();

                if (HeaderRegex?.IsMatch(header) ?? true)
                {
                    if (content != null)
                        throw new PropertyAlreadySetException();

                    content = initializeGetter.Invoke();

                    for (var index = fromIndex; index < values.Count(); index++)
                    {
                        var valueSetter = valuesSetters
                            .SingleOrDefault(s => s.Item1 == index);

                        valueSetter?.Item2.Invoke(values.ElementAt(index));
                    }
                }
                else
                {
                    if (!(currentChild?.HeaderRegex.IsMatch(header) ?? false)
                        && childs.Any(c => c.Item1.HeaderRegex.IsMatch(header)))
                    {
                        currentChild = childs
                            .SingleOrDefault(c => c.Item1.HeaderRegex.IsMatch(header))?.Item1;
                    }

                    currentChild?.Set(values);
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void CheckChilds()
        {
            var sameRecordHeaders = childs
                .Where(r => r.Item1.HeaderRegex != null)
                .GroupBy(r => r.Item1.HeaderRegex)
                .Where(g => g.Count() > 1).ToArray();

            if (sameRecordHeaders.Any())
                throw new SameRecordHeaderException(
                    sameRecordHeaders.First().First().Item1.HeaderRegex?.ToString());
        }

        private IEnumerable<Tuple<IDeserializer, Action>> GetChilds(Type type)
        {
            var classProperties = type.GetProperties()
                .Where(p => p.PropertyType.IsClassType()
                    || p.PropertyType.IsEnumerableType()).ToArray();

            foreach (var property in classProperties)
            {
                var isEnumerable = property.PropertyType.IsEnumerableType();

                var deserializer = isEnumerable
                    ? (IDeserializer)new EnumerableDeserializer(property.PropertyType)
                    : (IDeserializer)new ValueDeserializer(property.PropertyType);

                yield return new Tuple<IDeserializer, Action>(
                    item1: deserializer,
                    item2: () => property.SetValue(
                        obj: content,
                        value: deserializer.Get()));
            }
        }

        private Func<object> GetInitializeGetter(Type type)
        {
            return () => Activator.CreateInstance(type);
        }

        private IEnumerable<Tuple<int, Action<string>>> GetValueSetters(Type type)
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

            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(string))
                {
                    yield return new Tuple<int, Action<string>>(
                        item1: property.Index.Value,
                        item2: (text) => property.Property.SetText(
                            content: content,
                            text: text));
                }
                else if (property.PropertyType == typeof(DateTime))
                {
                    yield return new Tuple<int, Action<string>>(
                        item1: property.Index.Value,
                        item2: (text) => property.Property.SetDateTime(
                            content: content,
                            text: text));
                }
                else if (property.PropertyType == typeof(TimeSpan))
                {
                    yield return new Tuple<int, Action<string>>(
                        item1: property.Index.Value,
                        item2: (text) => property.Property.SetTimeSpan(
                            content: content,
                            text: text));
                }
                else
                {
                    yield return new Tuple<int, Action<string>>(
                        item1: property.Index.Value,
                        item2: (text) => property.Property.SetValue(
                            content: content,
                            text: text));
                }
            }
        }

        #endregion Private Methods
    }
}