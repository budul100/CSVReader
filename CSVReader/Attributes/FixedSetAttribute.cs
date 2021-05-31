using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class FixedSetAttribute
        : BaseSetAttribute
    {
        #region Public Constructors

        public FixedSetAttribute(string headerRegex = default, int headerLength = 0, string trimRegex = default)
            : base(headerRegex, false, trimRegex)
        {
            HeaderLength = headerLength;
        }

        #endregion Public Constructors

        #region Public Properties

        public int HeaderLength { get; }

        #endregion Public Properties
    }
}