using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ImportField : Attribute
    {
        #region Public Constructors

        public ImportField(int index)
        {
            Index = index;
        }

        public ImportField(string header)
        {
            Header = header;
        }

        #endregion Public Constructors

        #region Public Properties

        public string Header { get; }

        public int Index { get; }

        #endregion Public Properties
    }
}