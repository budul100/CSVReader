using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class TypeAttribute
        : BaseTypeAttribute
    {
        #region Public Constructors

        public TypeAttribute(string headerPattern = default, bool lastValueInfinite = false)
            : base(headerPattern, lastValueInfinite)
        { }

        #endregion Public Constructors
    }
}