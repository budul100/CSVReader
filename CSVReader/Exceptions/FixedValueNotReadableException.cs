using System;
using System.Runtime.Serialization;

namespace CSVReader.Exceptions
{
    public class FixedValueNotReadableException
        : Exception
    {
        #region Public Constructors

        public FixedValueNotReadableException(string line, int start)
            : this(GetMessage(line, start))
        { }

        public FixedValueNotReadableException()
        { }

        public FixedValueNotReadableException(string message)
            : base(message)
        { }

        public FixedValueNotReadableException(string message, Exception innerException)
            : base(message, innerException)
        { }

        #endregion Public Constructors

        #region Protected Constructors

        protected FixedValueNotReadableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        #endregion Protected Constructors

        #region Private Methods

        private static string GetMessage(string line, int start)
        {
            var result = $"A value starting from {start} cannot be read from the following line since it has {line?.Length ?? 0} characters only: {line}.";

            return result;
        }

        #endregion Private Methods
    }
}