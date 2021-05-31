using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class DelimitedFieldAttribute
        : BaseFieldAttribute
    {
        #region Public Constructors

        public DelimitedFieldAttribute(int index, int count = 1)
            : base(index)
        {
            if (count < 1)
            {
                throw new ArgumentException(
                    message: $"Length must be greater than 0",
                    paramName: nameof(count));
            }

            Count = count;
        }

        #endregion Public Constructors

        #region Public Properties

        public int Count { get; }

        #endregion Public Properties
    }
}