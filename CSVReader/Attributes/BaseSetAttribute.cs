using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public abstract class BaseSetAttribute
        : BaseTypeAttribute
    {
        #region Protected Constructors

        protected BaseSetAttribute(string headerRegex, string trimRegex, string delimiters, bool lastValueInfinite)
            : base(headerRegex, lastValueInfinite)
        {
            TrimRegex = trimRegex;
            Delimiters = delimiters;
        }

        #endregion Protected Constructors

        #region Public Properties

        public string Delimiters { get; }

        public string TrimRegex { get; }

        #endregion Public Properties
    }
}