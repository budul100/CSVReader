using System;

namespace CSVReader.Factories
{
    internal class ChildFactory
        : RecordFactory
    {
        #region Public Constructors

        public ChildFactory(Type type, bool trimValues, char[] valueSeparators, int? headerLength)
            : base(type, trimValues, valueSeparators, headerLength)
        { }

        #endregion Public Constructors

        #region Public Properties

        public Action<string> ContentsSetter { get; set; }

        #endregion Public Properties
    }
}