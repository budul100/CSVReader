using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class SetAttribute
        : TypeAttribute
    {
        #region Public Constructors

        public SetAttribute(string headerRegex = default, bool lastValueInfinite = false, string trimRegex = default, string delimiters = ",")
            : base(headerRegex, lastValueInfinite)
        {
            TrimRegex = trimRegex;
            Delimiters = delimiters;
        }

        #endregion Public Constructors

        #region Public Properties

        public string Delimiters { get; }

        public string TrimRegex { get; }

        #endregion Public Properties
    }
}