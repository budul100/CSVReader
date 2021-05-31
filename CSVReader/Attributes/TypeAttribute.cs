using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class TypeAttribute
        : BaseTypeAttribute
    {
        #region Public Constructors

        public TypeAttribute(string headerRegex = default, bool lastValueInfinite = false)
            : base(headerRegex, lastValueInfinite)
        { }

        #endregion Public Constructors
    }
}