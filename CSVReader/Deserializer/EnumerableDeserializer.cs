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
        private readonly Func<object> contentGetter;
        private readonly Type listType;

        private object content;
        private Action<object> valueSetter;

        #endregion Private Fields

        #region Public Constructors

        public EnumerableDeserializer(Type type, bool isList)
        {
            var headerRegex = type.GetAttribute<ImportRecord>().HeaderRegex;
            if (!string.IsNullOrWhiteSpace(headerRegex)) HeaderRegex = new Regex($"^{headerRegex}$");

            listType = typeof(List<>).MakeGenericType(type);
            addMethod = listType.GetMethod("Add");

            var getMethod = isList
                ? listType.GetMethod("ToList")
                : listType.GetMethod("ToArray");
            contentGetter = () => getMethod.Invoke(
                obj: content,
                parameters: null);

            child = new ValueDeserializer(type);

            Initialize();
        }

        #endregion Public Constructors

        #region Public Properties

        public object Content => contentGetter.Invoke();

        public Regex HeaderRegex { get; }

        #endregion Public Properties

        #region Public Methods

        public void Initialize()
        {
            child.Initialize();

            content = Activator.CreateInstance(listType);
            valueSetter = (value) => addMethod.Invoke(
                obj: content,
                parameters: new object[] { value });
        }

        public bool Set(string[] values)
        {
            var success = false;

            if (values?.Any() ?? false)
            {
                var header = values[0];

                if (HeaderRegex.IsMatch(header))
                {
                    child.Terminate();
                    child.Initialize();
                }

                success = child.Set(values);

                if (success && HeaderRegex.IsMatch(header))
                {
                    valueSetter.Invoke(child.Content);
                }
            }

            return success;
        }

        public void Terminate()
        {
            child.Terminate();
        }

        #endregion Public Methods
    }
}