using System;

namespace CSVReader.Factories
{
    internal class ChildFacade
    {
        #region Public Properties

        public Action<string> ContentsSetter { get; set; }

        public RecordFactory Factory { get; set; }

        #endregion Public Properties
    }
}