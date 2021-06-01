using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public abstract class BaseTypeAttribute
        : Attribute
    {
        #region Protected Constructors

        protected BaseTypeAttribute(string headerPattern = default, bool lastValueInfinite = false)
        {
            HeaderPattern = headerPattern;
            LastValueInfinite = lastValueInfinite;
        }

        #endregion Protected Constructors

        #region Public Properties

        public string HeaderPattern { get; }

        public bool LastValueInfinite { get; }

        #endregion Public Properties
    }
}