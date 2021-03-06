﻿using CSVReader.Attributes;
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

        private readonly string headerRegex;
        private readonly bool lastValueInfinite;
        private readonly Type recordType;
        private readonly bool trimValues;

        private IList fieldList;
        private Action fieldListSetter;

        #endregion Private Fields

        #region Public Constructors

        public RecordFactory(Type type, bool trimValues)
        {
            this.trimValues = trimValues;

            recordType = type.GetContentType();

            var typeAttribute = recordType.GetAttribute<TypeAttribute>();

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
            HeaderRegexes.Add(headerRegex);

            foreach (var childFactory in childFacades)
            {
                childFactory.Factory.CompleteInitialization();

                HeaderRegexes.UnionWith(childFactory.Factory.HeaderRegexes);
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

        public void InitializeByAttributes()
        {
            var fieldProperties = recordType.GetFieldProperties();

            foreach (var fieldProperty in fieldProperties)
            {
                var fieldAttribute = fieldProperty.GetAttribute<FieldAttribute>();

                var isLastInfinite = lastValueInfinite
                    && fieldProperty == fieldProperties.Last();

                CreateFieldSetter(
                    property: fieldProperty,
                    fieldIndex: fieldAttribute.Index,
                    fieldLength: fieldAttribute.Length,
                    isLastInfinite: isLastInfinite);
            }

            var childProperties = recordType.GetChildProperties();

            foreach (var itemProperty in childProperties)
            {
                var childFacade = GetChildFacade(itemProperty);
                childFacade.Factory.InitializeByAttributes();
            }
        }

        public void SetContents(IEnumerable<string> contents)
        {
            IsNewRecord = false;

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

                    relevantChild?.ContentsSetter.Invoke(contents);
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void AddChild(IList itemList, RecordFactory childFactory, IEnumerable<string> values)
        {
            childFactory.SetContents(values);

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

            var converted = trimValues && value != default
                ? value.Trim()
                : value;

            fieldList.Add(converted);

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

                var converted = trimValues && value != default
                    ? value.Trim().To(type)
                    : value.To(type);

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

        private void CreateFieldSetter(PropertyInfo property, int fieldIndex, int fieldLength, bool isLastInfinite)
        {
            var type = property.PropertyType.GetContentType();

            if (fieldLength == 1 && !isLastInfinite)
            {
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
            else
            {
                for (var columnIndex = fieldIndex; columnIndex < fieldIndex + fieldLength; columnIndex++)
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
        }

        private ChildFacade GetChildFacade(PropertyInfo property)
        {
            var itemType = property.PropertyType;

            var childFactory = new RecordFactory(
                type: itemType.GetContentType(),
                trimValues: trimValues);

            var result = new ChildFacade
            {
                Factory = childFactory,
            };

            if (itemType.IsEnumerableType())
            {
                var itemList = itemType.GetAsList();

                result.ContentsSetter = (values) => AddChild(itemList, childFactory, values);

                condenseSetters.Add(() => CondenseChildList(property, itemType, itemList));
            }
            else
            {
                result.ContentsSetter = (values) => SetChild(property, childFactory, values);
            }

            childFacades.Add(result);

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

        private void SetChild(PropertyInfo property, RecordFactory childFactory, IEnumerable<string> values)
        {
            if (property.GetValue(Record) != default)
            {
                throw new PropertyAlreadySetException($"The property {property.Name} can only be set once.");
            }

            childFactory.SetContents(values);

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

            var converted = trimValues && value != default
                ? value.Trim()
                : value;

            property.SetValue(
                obj: Record,
                value: converted);
        }

        private void SetValue(PropertyInfo property, Type type, string value)
        {
            if (value.IsConvertibleTo(type))
            {
                fieldListSetter?.Invoke();

                var converted = trimValues && value != default
                    ? value.Trim().To(type)
                    : value.To(type);

                property.SetValue(
                    obj: Record,
                    value: converted);
            }
        }

        #endregion Private Methods
    }
}