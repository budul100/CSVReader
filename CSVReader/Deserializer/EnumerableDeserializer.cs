using CSVReader.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CSVReader.Deserializers
{
    internal class EnumerableDeserializer : IDeserializer
    {
        #region Private Fields

        private readonly ValueDeserializer child;
        private readonly Func<object> contentGetter;
        private readonly Action itemsSetter;
        private object content;

        #endregion Private Fields

        #region Public Constructors

        public EnumerableDeserializer(Type type)
        {
            var contentType = type.GetContentType();

            HeaderRegex = contentType.GetHeaderRegex();
            child = new ValueDeserializer(contentType);

            var listType = contentType.GetListType();

            itemsSetter = GetItemsSetter(listType);
            contentGetter = GetContentGetter(
                type: type,
                listType: listType);
        }

        #endregion Public Constructors

        #region Public Properties

        public Regex HeaderRegex { get; }

        #endregion Public Properties

        #region Public Methods

        public object Get()
        {
            itemsSetter.Invoke();

            var result = contentGetter.Invoke();
            content = null;
            return result;
        }

        public void Set(IEnumerable<string> values)
        {
            if (HeaderRegex.IsMatch(values.First()))
            {
                itemsSetter.Invoke();
            }

            child.Set(values);
        }

        #endregion Public Methods

        #region Private Methods

        private Func<object> GetContentGetter(Type type, Type listType)
        {
            var enumerableType = type.GetContentType();

            var isList = enumerableType.IsGenericType
                && enumerableType.GetGenericTypeDefinition() == typeof(List<>);

            var getMethod = isList
                ? listType.GetMethod("ToList")
                : listType.GetMethod("ToArray");

            return () => getMethod.Invoke(
                obj: content,
                parameters: null);
        }

        private Action GetItemsSetter(Type listType)
        {
            var addMethod = listType.GetMethod("Add");

            return () =>
            {
                if (content == null)
                {
                    content = Activator.CreateInstance(listType);
                }
                else
                {
                    var item = child.Get();

                    if (item != null) addMethod.Invoke(
                        obj: content,
                        parameters: new object[] { item });
                }
            };
        }

        #endregion Private Methods
    }
}