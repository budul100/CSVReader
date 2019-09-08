using System;

namespace CSVReader.Exceptions
{
    public class SameRecordHeaderException : Exception
    {
        #region Public Constructors

        public SameRecordHeaderException(string header)
        {
            Header = header;
        }

        #endregion Public Constructors

        #region Public Properties

        public string Header { get; }

        #endregion Public Properties
    }
}