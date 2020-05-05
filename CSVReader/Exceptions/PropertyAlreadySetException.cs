using System;

namespace CSVReader.Exceptions
{
    public class PropertyAlreadySetException : Exception
    {
        #region Public Constructors

        public PropertyAlreadySetException(string message)
            : base(message)
        { }

        public PropertyAlreadySetException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public PropertyAlreadySetException()
        { }

        #endregion Public Constructors
    }
}