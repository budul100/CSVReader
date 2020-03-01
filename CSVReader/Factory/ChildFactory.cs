using CSVReader.Deserializers;
using CSVReader.Exceptions;
using CSVReader.Extensions;
using CSVReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSVReader.Factory
{
    internal class ChildFactory
    {
        #region Public Constructors

        public ChildFactory(Type type)
        {
            Childs = GetChilds(type).ToArray();

            CheckChilds();
        }

        #endregion Public Constructors

        #region Public Properties

        public bool AnyRegex { get; private set; }

        public IEnumerable<ChildDefinition> Childs { get; }

        #endregion Public Properties

        #region Private Methods

        private void CheckChilds()
        {
            var sameRecordHeaders = Childs
                .GroupBy(r => r.HeaderRegex)
                .Where(g => g.Count() > 1).ToArray();

            if (sameRecordHeaders.Any())
                throw new SameRecordHeaderException(
                    sameRecordHeaders.First().First().HeaderRegex?.ToString());
        }

        private IEnumerable<ChildDefinition> GetChilds(Type type)
        {
            var properties = type.GetChildProperties().ToArray();

            foreach (var property in properties)
            {
                var deserializer = property.PropertyType.IsClassEnumerable()
                    ? new EnumerableDeserializer(property.PropertyType) as BaseDeserializer
                    : new ValueDeserializer(property.PropertyType) as BaseDeserializer;

                var child = new ChildDefinition
                {
                    Deserializer = deserializer,
                    HeaderRegex = deserializer.HeaderRegex,
                    Setter = (content) => property.SetValue(
                        obj: content,
                        value: deserializer.Get()),
                };

                AnyRegex = AnyRegex
                    || deserializer.HeaderRegex != default;

                yield return child;
            }
        }

        #endregion Private Methods
    }
}