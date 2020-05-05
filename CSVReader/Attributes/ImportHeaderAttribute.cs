using System;

namespace CSVReader.Attributes
{
    public abstract class ImportHeaderAttribute
        : Attribute
    {
        #region Protected Constructors

        protected ImportHeaderAttribute(string headerRegex)
        {
            HeaderRegex = headerRegex;
        }

        #endregion Protected Constructors

        #region Public Properties

        public string HeaderRegex { get; }

        #endregion Public Properties
    }
}