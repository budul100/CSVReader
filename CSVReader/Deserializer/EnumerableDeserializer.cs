using CSVReader.Attributes;
using CSVReader.Extensions;
using CSVReader.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CSVReader.Deserializers
{
    internal class EnumerableDeserializer : IDeserializer
    {
        #region Private Fields

        private readonly MethodInfo addMethod;
        private readonly IDeserializer child;
        private readonly Type listType;
        private Action<object> valueSetter;

        #endregion Private Fields

        #region Public Constructors

        public EnumerableDeserializer(Type type)
        {
            var headerRegex = type.GetAttribute<ImportRecord>().HeaderRegex;
            if (!string.IsNullOrWhiteSpace(headerRegex)) HeaderRegex = new Regex($"^{headerRegex}$");

            listType = typeof(List<>).MakeGenericType(type);
            addMethod = listType.GetMethod("Add");

            child = new ValueDeserializer(type);

            Initialize();
        }

        #endregion Public Constructors

        #region Public Properties

        public object Content { get; private set; }

        public Regex HeaderRegex { get; }

        #endregion Public Properties

        #region Public Methods

        public void Initialize()
        {
            child.Initialize();

            Content = Activator.CreateInstance(listType);
            valueSetter = (value) => addMethod.Invoke(
                obj: Content,
                parameters: new object[] { value });
        }

        public bool Set(string[] values)
        {
            var success = false;

            if (values?.Any() ?? false)
            {
                var header = values[0];

                if (HeaderRegex.IsMatch(header))
                    child.Initialize();

                success = child.Set(values);

                if (success && HeaderRegex.IsMatch(header))
                {
                    valueSetter.Invoke(child.Content);
                }
            }

            return success;
        }

        #endregion Public Methods
    }
}