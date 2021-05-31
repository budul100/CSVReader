using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public abstract class BaseTypeAttribute
        : Attribute
    {
        #region Protected Constructors

        protected BaseTypeAttribute(string headerRegex = default, bool lastValueInfinite = false)
        {
            HeaderRegex = headerRegex;
            LastValueInfinite = lastValueInfinite;
        }

        #endregion Protected Constructors

        #region Public Properties

        public string HeaderRegex { get; }

        public bool LastValueInfinite { get; }

        #endregion Public Properties
    }
}