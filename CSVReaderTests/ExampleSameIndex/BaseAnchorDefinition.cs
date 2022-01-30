using CSVReader.Attributes;
using System.Collections.Generic;

namespace ExampleSameIndex
{
    public abstract class BaseAnchorDefinition
        : BaseValueDefinition
    {
        #region Public Properties

        [DelimitedField(0)]
        public string Abbreviation { get; set; }

        [DelimitedField(1)]
        public string Description { get; set; }

        [DelimitedField(2)]
        public virtual bool IncludingReverse { get; set; }

        public bool IsReverse { get; set; }

        [DelimitedField(3)]
        public override IEnumerable<string> Values { get; set; }

        #endregion Public Properties
    }
}