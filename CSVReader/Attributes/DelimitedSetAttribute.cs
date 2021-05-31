using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class DelimitedSetAttribute
        : BaseSetAttribute
    {
        #region Public Constructors

        public DelimitedSetAttribute(string headerRegex = default, bool lastValueInfinite = false, string trimRegex = default,
            string delimiters = ",")
            : base(headerRegex, lastValueInfinite, trimRegex)
        {
            Delimiters = delimiters;
        }

        #endregion Public Constructors

        #region Public Properties

        public string Delimiters { get; }

        #endregion Public Properties
    }
}