using System;

namespace CSVReader.Factories
{
    internal class ChildFactory
        : RecordFactory
    {
        #region Public Constructors

        public ChildFactory(Type type, bool trimValues, char[] valueSeparators)
            : base(type, trimValues, valueSeparators)
        { }

        #endregion Public Constructors

        #region Public Properties

        public Action<string> ContentsSetter { get; set; }

        #endregion Public Properties
    }
}