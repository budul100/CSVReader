using System;
using System.Reflection;

namespace CSVReader.Models
{
    internal class FieldDefinition
    {
        #region Public Properties

        public string Format { get; set; }

        public int Index { get; set; }

        public int Length { get; set; }

        public PropertyInfo Property { get; set; }

        public Type Type { get; set; }

        #endregion Public Properties
    }
}