using System;
using System.Collections.Generic;

namespace CSVReader.Factories
{
    internal class ChildFacade
    {
        #region Public Properties

        public Action<IEnumerable<string>> ContentsSetter { get; set; }

        public RecordFactory Factory { get; set; }

        #endregion Public Properties
    }
}