using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ImportRecord
        : ImportHeader
    {
        #region Public Constructors

        public ImportRecord(string headerRegex)
            : base(headerRegex)
        { }

        #endregion Public Constructors
    }
}