using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ImportFileAttribute
        : ImportHeaderAttribute
    {
        #region Public Constructors

        public ImportFileAttribute(string headerRegex, string delimiter = "\t")
            : base(headerRegex)
        {
            Delimiter = delimiter;
        }

        public ImportFileAttribute(string headerRegex, string trimRegex, string delimiter = "\t")
            : base(headerRegex)
        {
            TrimRegex = trimRegex;
            Delimiter = delimiter;
        }

        public ImportFileAttribute(string delimiter = "\t")
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