using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ImportFile
        : ImportHeader
    {
        #region Public Constructors

        public ImportFile(string headerRegex, string delimiter = "\t")
            : base(headerRegex)
        {
            Delimiter = delimiter;
        }

        public ImportFile(string headerRegex, string trimRegex, string delimiter = "\t")
            : base(headerRegex)
        {
            TrimRegex = trimRegex;
            Delimiter = delimiter;
        }

        public ImportFile(string delimiter = "\t")
            : this(default, delimiter)
        {
            Delimiter = delimiter;
        }

        #endregion Public Constructors

        #region Public Properties

        public string Delimiter { get; }

        public string TrimRegex { get; }

        #endregion Public Properties
    }
}