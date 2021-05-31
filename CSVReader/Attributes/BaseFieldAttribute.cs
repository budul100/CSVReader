using System;

namespace CSVReader.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public abstract class BaseFieldAttribute
        : Attribute
    {
        #region Protected Constructors

        protected BaseFieldAttribute(int index = 0)
        {
            Index = index;
        }

        #endregion Protected Constructors

        #region Public Properties

        public int Index { get; set; }

        #endregion Public Properties
    }
}