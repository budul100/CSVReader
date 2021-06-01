using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class DelimitedSetAttribute
        : BaseSetAttribute
    {
        #region Public Constructors

        public DelimitedSetAttribute(string headerPattern = default, string trimPattern = default,
            string delimiters = ",", bool lastValueInfinite = false)
            : base(headerPattern, trimPattern, delimiters, lastValueInfinite)
        { }

        #endregion Public Constructors
    }
}