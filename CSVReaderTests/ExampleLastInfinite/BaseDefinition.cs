using CSVReader.Attributes;
using System.Collections.Generic;

namespace CSVReaderTests.ExampleLastInfinite
{
    [Set(delimiters: ";", lastValueInfinite: true)]
    public class BaseDefinition
    {
        #region Public Properties

        [Field(0)]
        public string Abbreviation { get; set; }

        [Field(3)]
        public virtual IEnumerable<string> Anchors { get; set; }

        [Field(1)]
        public string Description { get; set; }

        [Field(2)]
        public bool IncludingReverse { get; set; }

        public bool IsReverse { get; set; }

        #endregion Public Properties
    }
}