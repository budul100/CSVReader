using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class FixedSetAttribute
        : BaseSetAttribute
    {
        #region Public Constructors

        public FixedSetAttribute(string headerPattern = default, string trimPattern = default)
            : base(headerPattern, trimPattern, default, false)
        { }

        #endregion Public Constructors
    }
}