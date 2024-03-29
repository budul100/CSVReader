﻿using CSVReader.Attributes;
using CSVReader.Exceptions;
using CSVReader.Extensions;
using RegexExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TB.ComponentModel;

namespace CSVReader.Factories
{
    internal class RecordFactory
        : IDisposable
    {
        #region Private Fields

        private readonly IList<ChildFactory> childFactories = new List<ChildFactory>();
        private readonly IList<Action> condenseSetters = new List<Action>();
        private readonly IDictionary<int, Action<string, bool>> fieldSetters = new Dictionary<int, Action<string, bool>>();
        private readonly string headerPattern;
        private readonly bool lastValueInfinite;
        private readonly Type recordType;
        private readonly bool trimValues;
        private readonly char[] valueSeparators;

        private Func<string, IEnumerable<string>> contentGetter;
        private IList fieldList;
        private Action fieldListSetter;
        private Func<string, Func<string, bool>> headerCheckGetter;

        #endregion Private Fields

        #region Public Constructors

        public RecordFactory(Type type, bool trimValues, char[] valueSeparators)
        {
            this.trimValues = trimValues;
            this.valueSeparators = valueSeparators;

            recordType = type.GetContentType();

            var typeAttribute = recordType.GetAttribute<BaseTypeAttribute>();

            lastValueInfinite = typeAttribute?.LastValueInfinite ?? false;
            headerPattern = typeAttribute?.HeaderPattern;

            if (!string.IsNullOrWhiteSpace(headerPattern))
            {
                HeaderPatterns.Add(headerPattern);
            }
        }

        #endregion Public Constructors

        #region Public Properties

        public HashSet<string> HeaderPatterns { get; } = new HashSet<string>();

        public bool IsNewRecord { get; private set; }

        public object Record { get; private set; }

        #endregion Public Properties

        #region Public Methods

        public void CondenseRecord()
        {
            if (Record != default)
            {
                foreach (var childFactory in childFactories)
                {
                    childFactory.CondenseRecord();
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
            headerCheckGetter = GetHeaderCheckGetterDelimiteds();

            var properties = recordType.GetDelimitedsProperties();

            contentGetter = (line) => line.GetContents(
                separator: valueSeparators,
                trimValues: trimValues);

            foreach (var property in properties)
            {
                var attribute = property.GetAttribute<DelimitedFieldAttribute>();

                if (attribute != default)
                {
                    var isLastInfinite = lastValueInfinite
                        && property == properties.Last();

                    if (fieldSetters.ContainsKey(attribute.Index))
                    {
                        throw new PropertyAlreadySetException(
                            $"There can be only one property with {typeof(DelimitedFieldAttribute)} " +
                            $"and index {attribute.Index} in {recordType}. ");
                    }

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

            foreach (var childProperty in childProperties)
            {
                var childFactory = GetChildFactory(childProperty);
                childFactory.InitializeDelimiteds();

                HeaderPatterns.UnionWith(childFactory.HeaderPatterns);
            }
        }

        public void InitializeFixeds()
        {
            headerCheckGetter = GetHeaderCheckGetterFixeds();

            var properties = recordType.GetFixedsProperties();
            var fixedsGetters = properties.GetFixedGetters().ToArray();

            contentGetter = (string line) => line.GetContents(
                getters: fixedsGetters,
                trimValues: trimValues);

            var index = 0;

            foreach (var property in properties)
            {
                CreateFieldSetterSingle(
                    property: property,
                    fieldIndex: index++);
            }

            var childProperties = recordType.GetChildProperties();

            foreach (var childProperty in childProperties)
            {
                var childFactory = GetChildFactory(childProperty);
                childFactory.InitializeFixeds();

                HeaderPatterns.UnionWith(childFactory.HeaderPatterns);
            }
        }

        public void SetContents(string line)
        {
            IsNewRecord = false;

            var headerChecker = headerCheckGetter.Invoke(line);

            var setFields = childFactories.Count == 0
                || (!string.IsNullOrWhiteSpace(headerPattern) && headerChecker.Invoke(headerPattern));

            if (Record == default || setFields)
            {
                RenewRecord();
            }

            if (setFields)
            {
                SetFields(line);
            }
            else if (childFactories.Count > 0)
            {
                var relevant = childFactories
                    .SingleOrDefault(c => c.HeaderPatterns.Any(r => headerChecker(r)));

                relevant?.ContentsSetter.Invoke(line);
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
                    value: (value, _) => SetText(property, value));
            }
            else
            {
                fieldSetters.Add(
                    key: fieldIndex,
                    value: (value, _) => SetValue(property, type, value));
            }
        }

        private ChildFactory GetChildFactory(PropertyInfo property)
        {
            var itemType = property.PropertyType;

            var result = new ChildFactory(
                type: itemType.GetContentType(),
                trimValues: trimValues,
                valueSeparators: valueSeparators);

            if (itemType.IsEnumerableType())
            {
                var itemList = itemType.GetAsList();

                result.ContentsSetter = (line) => AddChild(itemList, result, line);

                condenseSetters.Add(() => CondenseChildList(property, itemType, itemList));
            }
            else
            {
                result.ContentsSetter = (line) => SetChild(property, result, line);
            }

            childFactories.Add(result);

            return result;
        }

        private Func<string, Func<string, bool>> GetHeaderCheckGetterDelimiteds()
        {
            var headerRegex = new Regex($"^[^{new string(valueSeparators)}]+");

            Func<string, bool> result(string line)
            {
                var header = headerRegex.Match(line).Value;

                bool check(string pattern)
                {
                    return header.IsMatchOrEmptyPattern(pattern);
                }

                return check;
            }

            return result;
        }

        private Func<string, Func<string, bool>> GetHeaderCheckGetterFixeds()
        {
            Func<string, bool> result(string line)
            {
                bool check(string pattern)
                {
                    var frontPattern = pattern.GetFrontPattern();

                    return line.IsMatchOrEmptyPattern(frontPattern);
                }

                return check;
            }

            return result;
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

        private void SetFields(string line)
        {
            if (fieldSetters.Count > 0)
            {
                var contents = contentGetter
                    .Invoke(line).ToArray();

                if (contents?.Any() ?? false)
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