using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public abstract class BaseSetAttribute
        : BaseTypeAttribute
    {
        #region Protected Constructors

        protected BaseSetAttribute(string headerPattern, string trimPattern, string delimiters, bool lastValueInfinite)
            : base(headerPattern, lastValueInfinite)
        {
            Delimiters = delimiters;
            TrimPattern = trimPattern;
        }

        #endregion Protected Constructors

        #region Public Properties

        public string Delimiters { get; }

        public string TrimPattern { get; }

        #endregion Public Properties
    }
}