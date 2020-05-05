using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ImportRecordAttribute
        : ImportHeaderAttribute
    {
        #region Public Constructors

        public ImportRecordAttribute(string headerRegex)
            : base(headerRegex)
        { }

        public ImportRecordAttribute()
            : this(default)
        { }

        #endregion Public Constructors
    }
}