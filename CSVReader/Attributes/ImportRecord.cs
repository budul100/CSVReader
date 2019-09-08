using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ImportRecord : Attribute
    {
        #region Public Constructors

        public ImportRecord(string headerRegex)
        {
            HeaderRegex = headerRegex;
        }

        #endregion Public Constructors

        #region Public Properties

        public string HeaderRegex { get; }

        #endregion Public Properties
    }
}