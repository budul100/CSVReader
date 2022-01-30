using CSVReader.Attributes;
using System.Collections.Generic;

namespace ExampleSameIndex
{
    public abstract class BaseValueDefinition
    {
        #region Public Fields

        public const string Delimiters = ";";

        #endregion Public Fields

        #region Public Properties

        [DelimitedField(0)]
        public virtual string Identifier { get; set; }

        [DelimitedField(1)]
        public virtual IEnumerable<string> Values { get; set; }

        #endregion Public Properties

        #region Public Methods

        public U ShallowCopy<U>()
            where U : BaseValueDefinition
        {
            return MemberwiseClone() as U;
        }

        #endregion Public Methods
    }
}