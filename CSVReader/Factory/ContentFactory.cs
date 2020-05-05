using CSVReader.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSVReader.Factory
{
    internal class ContentFactory
    {
        #region Private Fields

        private readonly Type type;

        #endregion Private Fields

        #region Public Constructors

        public ContentFactory(Type type)
        {
            this.type = type;

            Setters = new Dictionary<int, Action<object, string>>();
            AddSetters();
        }

        #endregion Public Constructors

        #region Public Properties

        public Func<object> Getter => () => Activator.CreateInstance(type);

        public IDictionary<int, Action<object, string>> Setters { get; }

        #endregion Public Properties

        #region Private Methods

        private void AddSetters()
        {
            var fieldDescriptions = type.GetFieldDefinitions()
                .OrderBy(d => d.Index).ToArray();

            foreach (var fieldDescription in fieldDescriptions)
            {
                var isEnumerable = fieldDescription.Type.IsEnumerableType();
                var length = isEnumerable ? fieldDescription.Length : 1;

                for (var position = 0; position < length; position++)
                {
                    var setter = default(Action<object, string>);

                    if (isEnumerable)
                    {
                        var listType = fieldDescription.Type.GetContentType().GetListType();

                        if (fieldDescription.Type.GetContentType() == typeof(string))
                        {
                            setter = (content, text) => fieldDescription.Property.AddText(
                                listType: listType,
                                content: content,
                                text: text);
                        }
                        else
                        {
                            setter = (content, text) => fieldDescription.Property.AddValue(
                                listType: listType,
                                content: content,
                                text: text);
                        }
                    }
                    else if (fieldDescription.Type == typeof(DateTime) || fieldDescription.Type == typeof(DateTime?))
                    {
                        setter = (content, text) => fieldDescription.Property.SetDateTime(
                            content: content,
                            text: text,
                            format: fieldDescription.Format);
                    }
                    else if (fieldDescription.Type == typeof(TimeSpan) || fieldDescription.Type == typeof(TimeSpan?))
                    {
                        setter = (content, text) => fieldDescription.Property.SetTimeSpan(
                            content: content,
                            text: text,
                            format: fieldDescription.Format);
                    }
                    else if (fieldDescription.Type == typeof(string))
                    {
                        setter = (content, text) => fieldDescription.Property.SetText(
                            content: content,
                            text: text);
                    }
                    else
                    {
                        setter = (content, text) => fieldDescription.Property.SetValue(
                            content: content,
                            text: text);
                    }

                    if (setter != default)
                    {
                        Setters.Add(
                            key: fieldDescription.Index + position,
                            value: setter);
                    }
                }
            }
        }

        #endregion Private Methods
    }
}