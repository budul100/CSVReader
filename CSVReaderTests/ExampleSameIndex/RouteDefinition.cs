using CSVReader.Attributes;
using System.Collections.Generic;

namespace ExampleSameIndex
{
    [DelimitedSet(
        delimiters: Delimiters,
        lastValueInfinite: true)]
    public class RouteDefinition
        : BaseAnchorDefinition
    {
        #region Public Properties

        [DelimitedField(3)]
        public bool? IsBackward { get; set; }

        [DelimitedField(4)]
        public override IEnumerable<string> Values { get; set; }

        #endregion Public Properties
    }
}