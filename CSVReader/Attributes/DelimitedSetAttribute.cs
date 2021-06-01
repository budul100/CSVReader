using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class DelimitedSetAttribute
        : BaseSetAttribute
    {
        #region Public Constructors

        public DelimitedSetAttribute(string headerRegex = default, string trimRegex = default, string delimiters = ",",
            bool lastValueInfinite = false)
            : base(headerRegex, trimRegex, delimiters, lastValueInfinite)
        { }

        #endregion Public Constructors
    }
}