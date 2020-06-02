using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TypeAttribute
        : Attribute
    {
        #region Public Constructors

        public TypeAttribute(string headerRegex = default, bool lastValueInfinite = false)
        {
            HeaderRegex = headerRegex;
            LastValueInfinite = lastValueInfinite;
        }

        #endregion Public Constructors

        #region Public Properties

        public string HeaderRegex { get; }

        public bool LastValueInfinite { get; }

        #endregion Public Properties
    }
}