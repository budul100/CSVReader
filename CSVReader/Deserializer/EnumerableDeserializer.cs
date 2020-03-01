using CSVReader.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSVReader.Deserializers
{
    internal class EnumerableDeserializer
        : BaseDeserializer
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

        #region Public Methods

        public override object Get()
        {
            itemsSetter.Invoke();

            var result = contentGetter.Invoke();
            content = default;

            return result;
        }

        public override void Set(IEnumerable<string> values)
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
                parameters: default);
        }

        private Action GetItemsSetter(Type listType)
        {
            var addMethod = listType.GetMethod("Add");

            return () =>
            {
                if (content == default)
                {
                    content = Activator.CreateInstance(listType);
                }
                else
                {
                    var item = child.Get();

                    if (item != default)
                    {
                        addMethod.Invoke(
                            obj: content,
                            parameters: new object[] { item });
                    }
                }
            };
        }

        #endregion Private Methods
    }
}