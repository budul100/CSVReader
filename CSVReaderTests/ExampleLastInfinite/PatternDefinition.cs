using CSVReader.Attributes;
using System.Collections.Generic;

namespace CSVReaderTests.ExampleLastInfinite
{
    [Set(delimiters: ";", lastValueInfinite: true)]
    public class PatternDefinition
    {
        #region Public Properties

        [Field(0)]
        public string Abbreviation { get; set; }

        [Field(3)]
        public IEnumerable<string> Anchors { get; set; }

        [Field(1)]
        public string Description { get; set; }

        [Field(2)]
        public int Index { get; set; }

        #endregion Public Properties
    }
}