using System;
using System.Runtime.Serialization;

namespace CSVReader.Exceptions
{
    public class MissingHeaderPatternException
        : Exception
    {
        #region Public Constructors

        public MissingHeaderPatternException(Type type)
            : this(GetMessage(type))
        { }

        public MissingHeaderPatternException(string message)
            : base(message)
        { }

        public MissingHeaderPatternException(string message, Exception innerException)
            : base(message, innerException)
        { }

        public MissingHeaderPatternException()
        { }

        #endregion Public Constructors

        #region Protected Constructors

        protected MissingHeaderPatternException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        #endregion Protected Constructors

        #region Private Methods

        private static string GetMessage(Type type)
        {
            var result = $"At least one child object of '{type.Name}' has a header pattern defined. Therefore, '{type.Name}' must have a header pattern too.";

            return result;
        }

        #endregion Private Methods
    }
}