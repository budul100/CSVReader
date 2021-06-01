using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class FixedSetAttribute
        : BaseSetAttribute
    {
        #region Public Constructors

        public FixedSetAttribute(string headerRegex = default, string trimRegex = default)
            : base(headerRegex, trimRegex, default, false)
        { }

        #endregion Public Constructors
    }
}