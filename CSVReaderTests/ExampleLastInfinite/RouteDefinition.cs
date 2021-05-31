using CSVReader.Attributes;
using System.Collections.Generic;

namespace CSVReaderTests.ExampleLastInfinite
{
    [DelimitedSet(delimiters: ";", lastValueInfinite: true)]
    public class RouteDefinition
        : BaseDefinition
    {
        #region Public Properties

        [DelimitedField(4)]
        public override IEnumerable<string> Anchors { get; set; }

        [DelimitedField(3)]
        public bool? IsBackward { get; set; }

        #endregion Public Properties
    }
}