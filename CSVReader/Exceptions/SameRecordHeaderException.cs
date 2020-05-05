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

        public SameRecordHeaderException()
        {
        }

        public SameRecordHeaderException(string message, Exception innerException) : base(message, innerException)
        {
        }

        #endregion Public Properties
    }
}