using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class FieldAttribute
        : Attribute
    {
        #region Public Constructors

        public FieldAttribute(int index, int length = 1)
        {
            if (length < 1)
                throw new ArgumentException(
                    message: $"Length must be greater than 0",
                    paramName: nameof(length));

            Index = index;
            Length = length;
        }

        public FieldAttribute(string header)
        {
            Header = header;
        }

        #endregion Public Constructors

        #region Public Properties

        public string Header { get; }

        public int Index { get; }

        public int Length { get; } = 1;

        #endregion Public Properties
    }
}