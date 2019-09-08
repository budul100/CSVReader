using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ImportRecord : Attribute
    {
        #region Public Constructors

        public ImportRecord(string header)
        {
            Header = header;
        }

        #endregion Public Constructors

        #region Public Properties

        public string Header { get; }

        #endregion Public Properties
    }
}