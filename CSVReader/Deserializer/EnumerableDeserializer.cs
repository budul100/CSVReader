using CSVReader.Attributes;
using CSVReader.Extensions;
using CSVReader.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSVReader.Deserializers
{
    internal class EnumerableDeserializer : IDeserializer
    {
        #region Private Fields

        private readonly IDeserializer child;
        private readonly MethodInfo addMethod;
        private readonly Type listType;
        private Action<object> valueSetter;

        #endregion Private Fields

        #region Public Constructors

        public EnumerableDeserializer(Type type)
        {
            Header = type.GetAttribute<ImportRecord>().Header;

            listType = typeof(List<>).MakeGenericType(type);
            addMethod = listType.GetMethod("Add");

            child = new ValueDeserializer(type);

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
                if (values.First() == Header)
                    child.Initialize();

                success = child.Set(values);

                if (success && values.First() == Header)
                {
                    valueSetter.Invoke(child.Content);
                }
            }

            return success;
        }

        #endregion Public Methods
    }
}