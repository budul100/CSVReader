using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class ImportFieldAttribute
        : Attribute
    {
        #region Public Constructors

        public ImportFieldAttribute(int index, string format = default)
        {
            Index = index;
            Format = format;
        }

        public ImportFieldAttribute(int index, int length, string format = default)
        {
            Index = index;
            Format = format;
            Length = length;
        }

        public ImportFieldAttribute(string header, string format = default)
        {
            Header = header;
            Format = format;
        }

        #endregion Public Constructors

        #region Public Properties

        public string Format { get; }

        public string Header { get; }

        public int Index { get; }

        public int? Length { get; }

        #endregion Public Properties
    }
}