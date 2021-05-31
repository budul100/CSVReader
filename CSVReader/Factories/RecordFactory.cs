using CSVReader.Attributes;
using CSVReader.Exceptions;
using CSVReader.Extensions;
using RegexExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TB.ComponentModel;

namespace CSVReader.Factories
{
    internal class RecordFactory
        : IDisposable
    {
        #region Private Fields

        private readonly IList<ChildFacade> childFacades = new List<ChildFacade>();
        private readonly IList<Action> condenseSetters = new List<Action>();
        private readonly IDictionary<int, Action<string, bool>> fieldSetters = new Dictionary<int, Action<string, bool>>();
        private readonly int? headerLength;
        private readonly string headerRegex;
        private readonly bool lastValueInfinite;
        private readonly Type recordType;
        private readonly bool trimValues;
        private readonly char[] valueSeparators;

        private Func<string, IEnumerable<string>> contentGetter;
        private IList fieldList;
        private Action fieldListSetter;
        private IEnumerable<Func<string, string>> fixedsGetters;

        #endregion Private Fields

        #region Public Constructors

        public RecordFactory(Type type, bool trimValues, char[] valueSeparators, int? headerLength)
        {
            this.trimValues = trimValues;
            this.valueSeparators = valueSeparators;
            this.headerLength = headerLength;

            recordType = type.GetContentType();

            var typeAttribute = recordType.GetAttribute<BaseTypeAttribute>();

            lastValueInfinite = typeAttribute?.LastValueInfinite ?? false;
            headerRegex = typeAttribute?.HeaderRegex;
        }

        #endregion Public Constructors

        #region Public Properties

        public HashSet<string> HeaderRegexes { get; private set; } = new HashSet<string>();

        public bool IsNewRecord { get; private set; }

        public object Record { get; set; }

        #endregion Public Properties

        #region Public Methods

        public void CompleteInitialization()
        {
            if (!string.IsNullOrWhiteSpace(headerRegex))
            {
                HeaderRegexes.Add(headerRegex);
            }

            foreach (var childFacade in childFacades)
            {
                childFacade.Factory.CompleteInitialization();

                HeaderRegexes.UnionWith(childFacade.Factory.HeaderRegexes);
            }
        }

        public void CondenseRecord()
        {
            if (Record != default)
            {
                foreach (var childFactory in childFacades)
                {
                    childFactory.Factory.CondenseRecord();
                }

                foreach (var condenseSetter in condenseSetters)
                {
                    condenseSetter.Invoke();
                }
            }
        }

        public void Dispose()
        {
            CondenseRecord();
        }

        public void InitializeDelimiteds()
        {
            var properties = recordType.GetDelimitedsProperties();

            contentGetter = GetContentGetterDelimiteds();

            foreach (var property in properties)
            {
                var attribute = property.GetAttribute<DelimitedFieldAttribute>();

                if (attribute != default)
                {
                    var isLastInfinite = lastValueInfinite
                        && property == properties.Last();

                    if (attribute.Count > 1 || isLastInfinite)
                    {
                        CreateFieldSetterCollection(
                            property: property,
                            fieldIndex: attribute.Index,
                            fieldCount: attribute.Count);
                    }
                    else
                    {
                        CreateFieldSetterSingle(
                            property: property,
                            fieldIndex: attribute.Index);
                    }
                }
            }

            var childProperties = recordType.GetChildProperties();

            foreach (var itemProperty in childProperties)
            {
                var childFacade = GetChildFacade(itemProperty);
                childFacade.Factory.InitializeDelimiteds();
            }
        }

        public void InitializeFixeds()
        {
            var properties = recordType.GetFixedsProperties();

            fixedsGetters = GetFixedGetters(properties).ToArray();
            contentGetter = GetContentGetterFixeds();

            var index = (headerLength ?? 0) > 0 ? 1 : 0;

            foreach (var property in properties)
            {
                CreateFieldSetterSingle(
                    property: property,
                    fieldIndex: index++);
            }

            var childProperties = recordType.GetChildProperties();

            foreach (var itemProperty in childProperties)
            {
                var childFacade = GetChildFacade(itemProperty);
                childFacade.Factory.InitializeFixeds();
            }
        }

        public void SetContents(string line)
        {
            IsNewRecord = false;

            var contents = contentGetter
                .Invoke(line).ToArray();

            if (contents?.Any() ?? false)
            {
                if (contents.First().IsMatchOrEmptyPattern(headerRegex))
                {
                    RenewRecord();
                }

                if (contents.First().IsMatchOrEmptyPattern(headerRegex)
                    && fieldSetters.Any())
                {
                    SetFields(contents);
                }
                else if (childFacades.Any())
                {
                    var relevantChild = childFacades
                        .Where(c => c.Factory.HeaderRegexes.Any(r => contents.First().IsMatchOrEmptyPattern(r)))
                        .SingleOrDefault();

                    relevantChild?.ContentsSetter.Invoke(line);
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void AddChild(IList itemList, RecordFactory childFactory, string line)
        {
            childFactory.SetContents(line);

            if (childFactory.IsNewRecord)
            {
                itemList.Add(childFactory.Record);
            }
        }

        private void AddText(PropertyInfo property, string value, bool isLastValue)
        {
            if (fieldList == default)
            {
                RenewFieldList(property);
            }

            fieldList.Add(value);

            if (isLastValue)
            {
                fieldListSetter?.Invoke();
            }
        }

        private void AddValue(PropertyInfo property, Type type, string value, bool isLastValue)
        {
            if (value.IsConvertibleTo(type))
            {
                if (fieldList == default)
                {
                    RenewFieldList(property);
                }

                var converted = value.To(type);

                fieldList.Add(converted);
            }

            if (isLastValue)
            {
                fieldListSetter?.Invoke();
            }
        }

        private void CondenseChildList(PropertyInfo property, Type itemType, IList itemList)
        {
            if (Record != default)
            {
                var getMethod = itemType.IsListType()
                    ? itemList.GetType().GetMethod("ToList")
                    : itemList.GetType().GetMethod("ToArray");

                var value = getMethod.Invoke(
                    obj: itemList,
                    parameters: default);

                property.SetValue(
                    obj: Record,
                    value: value);

                itemList.Clear();
            }
        }

        private void CondenseFieldList(PropertyInfo property)
        {
            if (fieldList != default)
            {
                var getMethod = property.PropertyType.IsListType()
                    ? fieldList.GetType().GetMethod("ToList")
                    : fieldList.GetType().GetMethod("ToArray");

                var value = getMethod.Invoke(
                    obj: fieldList,
                    parameters: default);

                property.SetValue(
                    obj: Record,
                    value: value);
            }

            fieldList = default;
            fieldListSetter = default;
        }

        private void CreateFieldSetterCollection(PropertyInfo property, int fieldIndex, int fieldCount)
        {
            var type = property.PropertyType.GetContentType();

            for (var columnIndex = fieldIndex; columnIndex < fieldIndex + fieldCount; columnIndex++)
            {
                if (type == typeof(string))
                {
                    fieldSetters.Add(
                        key: columnIndex,
                        value: (value, isLastValue) => AddText(property, value, isLastValue));
                }
                else
                {
                    fieldSetters.Add(
                        key: columnIndex,
                        value: (value, isLastValue) => AddValue(property, type, value, isLastValue));
                }
            }
        }

        private void CreateFieldSetterSingle(PropertyInfo property, int fieldIndex)
        {
            var type = property.PropertyType.GetContentType();

            if (type == typeof(string))
            {
                fieldSetters.Add(
                    key: fieldIndex,
                    value: (value, isLastValue) => SetText(property, value));
            }
            else
            {
                fieldSetters.Add(
                    key: fieldIndex,
                    value: (value, isLastValue) => SetValue(property, type, value));
            }
        }

        private ChildFacade GetChildFacade(PropertyInfo property)
        {
            var itemType = property.PropertyType;

            var childFactory = new RecordFactory(
                type: itemType.GetContentType(),
                trimValues: trimValues,
                valueSeparators: valueSeparators,
                headerLength: headerLength);

            var result = new ChildFacade
            {
                Factory = childFactory,
            };

            if (itemType.IsEnumerableType())
            {
                var itemList = itemType.GetAsList();

                result.ContentsSetter = (line) => AddChild(itemList, childFactory, line);

                condenseSetters.Add(() => CondenseChildList(property, itemType, itemList));
            }
            else
            {
                result.ContentsSetter = (line) => SetChild(property, childFactory, line);
            }

            childFacades.Add(result);

            return result;
        }

        private IEnumerable<string> GetContentDelimiteds(string line)
        {
            var result = line.Split(valueSeparators);

            if (trimValues)
            {
                result = result
                    .Select(v => v.Trim()).ToArray();
            }

            return result;
        }

        private IEnumerable<string> GetContentFixeds(string line)
        {
            if (fixedsGetters?.Any() ?? false)
            {
                foreach (var fixedsGetter in fixedsGetters)
                {
                    var result = fixedsGetter.Invoke(line);

                    if (trimValues)
                    {
                        result = result.Trim();
                    }

                    yield return result;
                }
            }
        }

        private Func<string, IEnumerable<string>> GetContentGetterDelimiteds()
        {
            IEnumerable<string> result(string line) => GetContentDelimiteds(line);

            return result;
        }

        private Func<string, IEnumerable<string>> GetContentGetterFixeds()
        {
            IEnumerable<string> result(string line) => GetContentFixeds(line);

            return result;
        }

        private IEnumerable<Func<string, string>> GetFixedGetters(IEnumerable<PropertyInfo> properties)
        {
            if ((headerLength ?? 0) > 0)
            {
                string result(string line) => line.Substring(
                    startIndex: 0,
                    length: headerLength.Value);

                yield return result;
            }

            if (properties?.Any() ?? false)
            {
                foreach (var property in properties)
                {
                    var attribute = property.GetAttribute<FixedFieldAttribute>();

                    if (attribute != default)
                    {
                        string result(string line) => line.Substring(
                            startIndex: attribute.Start,
                            length: attribute.Length);

                        yield return result;
                    }
                }
            }
        }

        private void RenewFieldList(PropertyInfo property)
        {
            fieldListSetter?.Invoke();

            fieldList = property.PropertyType.GetAsList();

            fieldListSetter = () => CondenseFieldList(property);
        }

        private void RenewRecord()
        {
            CondenseRecord();

            Record = Activator.CreateInstance(recordType);
            IsNewRecord = true;
        }

        private void SetChild(PropertyInfo property, RecordFactory childFactory, string line)
        {
            if (property.GetValue(Record) != default)
            {
                throw new PropertyAlreadySetException($"The property {property.Name} can only be set once.");
            }

            childFactory.SetContents(line);

            if (childFactory.IsNewRecord)
            {
                property.SetValue(
                    obj: Record,
                    value: childFactory.Record);
            }
        }

        private void SetFields(IEnumerable<string> contents)
        {
            var setterIndex = 0;
            var lastSetterIndex = fieldSetters.Last().Key;

            foreach (var value in contents)
            {
                var isLastValue = value == contents.Last();

                if (fieldSetters.ContainsKey(setterIndex))
                {
                    fieldSetters[setterIndex].Invoke(
                        arg1: value,
                        arg2: isLastValue);
                }
                else if (lastValueInfinite
                    && setterIndex > lastSetterIndex)
                {
                    fieldSetters[lastSetterIndex].Invoke(
                        arg1: value,
                        arg2: isLastValue);
                }

                setterIndex++;
            }
        }

        private void SetText(PropertyInfo property, string value)
        {
            fieldListSetter?.Invoke();

            property.SetValue(
                obj: Record,
                value: value);
        }

        private void SetValue(PropertyInfo property, Type type, string value)
        {
            if (value.IsConvertibleTo(type))
            {
                fieldListSetter?.Invoke();

                var converted = value.To(type);

                property.SetValue(
                    obj: Record,
                    value: converted);
            }
        }

        #endregion Private Methods
    }
}