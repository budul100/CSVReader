using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public abstract class BaseSetAttribute
        : BaseTypeAttribute
    {
        #region Protected Constructors

        protected BaseSetAttribute(string headerRegex = default, bool lastValueInfinite = false, string trimRegex = default)
            : base(headerRegex, lastValueInfinite)
        {
            TrimRegex = trimRegex;
        }

        #endregion Protected Constructors

        #region Public Properties

        public string TrimRegex { get; }

        #endregion Public Properties
    }
}