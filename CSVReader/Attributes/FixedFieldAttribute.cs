using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class FixedFieldAttribute
        : BaseFieldAttribute
    {
        #region Public Constructors

        public FixedFieldAttribute(int start, int length)
        {
            if (length < 1)
            {
                throw new ArgumentException(
                    message: $"Length must be greater than 0",
                    paramName: nameof(length));
            }

            Start = start;
            Length = length;
        }

        #endregion Public Constructors

        #region Public Properties

        public int Length { get; }

        public int Start { get; }

        #endregion Public Properties
    }
}