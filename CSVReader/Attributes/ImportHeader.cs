using System;

namespace CSVReader.Attributes
{
    public abstract class ImportHeader
        : Attribute
    {
        #region Protected Constructors

        protected ImportHeader(string headerRegex)
        {
            HeaderRegex = headerRegex;
        }

        #endregion Protected Constructors

        #region Public Properties

        public string HeaderRegex { get; }

        #endregion Public Properties
    }
}