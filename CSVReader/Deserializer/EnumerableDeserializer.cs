using CSVReader.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CSVReader.Deserializers
{
    internal class EnumerableDeserializer
        : BaseDeserializer
    {
        #region Private Fields

        private readonly ValueDeserializer child;
        private readonly Func<object, object> contentGetter;
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
            contentGetter = type.GetContentGetter(listType);
        }

        #endregion Public Constructors

        #region Public Methods

        public override object Get()
        {
            itemsSetter.Invoke();

            var result = contentGetter.Invoke(content);
            content = default;

            return result;
        }

        public override void Set(IEnumerable<string> values)
        {
            if (HeaderRegex?.IsMatch(values.First()) ?? true)
            {
                itemsSetter.Invoke();
            }

            child.Set(values);
        }

        #endregion Public Methods

        #region Private Methods

        private void AddItem(Type listType, MethodInfo addMethod)
        {
            if (content == default)
            {
                content = Activator.CreateInstance(listType);
            }

            var item = child.Get();

            if (item != default)
            {
                addMethod.Invoke(
                    obj: content,
                    parameters: new object[] { item });
            }
        }

        private Action GetItemsSetter(Type listType)
        {
            var addMethod = listType.GetMethod("Add");

            return () => AddItem(
                listType: listType,
                addMethod: addMethod);
        }

        #endregion Private Methods
    }
}