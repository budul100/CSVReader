using CSVReader.Exceptions;
using CSVReader.Extensions;
using CSVReader.Factory;
using CSVReader.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSVReader.Deserializers
{
    internal class ValueDeserializer
        : BaseDeserializer
    {
        #region Private Fields

        private readonly bool anyRegex;
        private readonly IEnumerable<ChildDefinition> childs;
        private readonly Func<object> contentGetter;
        private readonly int fromIndex;
        private readonly IDictionary<int, Action<object, string>> valueSetters;

        private object content;
        private ChildDefinition currentChild;

        #endregion Private Fields

        #region Public Constructors

        public ValueDeserializer(Type type)
        {
            HeaderRegex = type.GetHeaderRegex();
            fromIndex = HeaderRegex == default ? 0 : 1;

            var childFactory = new ChildFactory(type);
            childs = childFactory.Childs;
            anyRegex = childFactory.AnyRegex;

            if (!anyRegex)
                currentChild = childFactory.Childs.FirstOrDefault();

            var contentFactory = new ContentFactory(type);
            contentGetter = contentFactory.Getter;
            valueSetters = contentFactory.Setters;
        }

        #endregion Public Constructors

        #region Public Methods

        public override object Get()
        {
            foreach (var child in childs)
            {
                if (content != default)
                    child.Setter.Invoke(content);
            }

            var result = content;
            content = default;

            return result;
        }

        public override void Set(IEnumerable<string> values)
        {
            if (values?.Any() ?? false)
            {
                var header = values.First();

                if ((HeaderRegex?.IsMatch(header) ?? false)
                    || !childs.Any())
                {
                    if (content != default)
                        throw new PropertyAlreadySetException();

                    content = contentGetter.Invoke();

                    for (var index = fromIndex; index < values.Count(); index++)
                    {
                        if (valueSetters.ContainsKey(index))
                        {
                            valueSetters[index]?.Invoke(
                                arg1: content,
                                arg2: values.ElementAt(index));
                        }
                    }
                }
                else
                {
                    if (content == default)
                        content = contentGetter.Invoke();

                    if (anyRegex &&
                        !(currentChild?.HeaderRegex?.IsMatch(header) ?? false))
                    {
                        currentChild = childs
                            .FirstOrDefault(c => c.HeaderRegex?.IsMatch(header) ?? true);
                    }

                    currentChild?.Deserializer.Set(values);
                }
            }
        }

        #endregion Public Methods
    }
}