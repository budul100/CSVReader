using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class ImportField
        : Attribute
    {
        #region Public Constructors

        public ImportField(int index, string format = default)
        {
            Index = index;
            Format = format;
        }

        public ImportField(string header, string format = default)
        {
            Header = header;
            Format = format;
        }

        #endregion Public Constructors

        #region Public Properties

        public string Format { get; }

        public string Header { get; }

        public int Index { get; }

        #endregion Public Properties
    }
}