using CSVReader.Attributes;
using System.Collections.Generic;

namespace CSVReaderTests.ExampleLastInfinite
{
    [DelimitedSet(delimiters: ";", lastValueInfinite: true)]
    public class BaseDefinition
    {
        #region Public Properties

        [DelimitedField(0)]
        public string Abbreviation { get; set; }

        [DelimitedField(3)]
        public virtual IEnumerable<string> Anchors { get; set; }

        [DelimitedField(1)]
        public string Description { get; set; }

        [DelimitedField(2)]
        public bool IncludingReverse { get; set; }

        public bool IsReverse { get; set; }

        #endregion Public Properties
    }
}