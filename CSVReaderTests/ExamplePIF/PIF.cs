using CSVReader.Attributes;
using System.Collections.Generic;

namespace ExamplePIF
{
    [DelimitedSet(headerRegex: "PEX|PIF", delimiters: "\t")]
    public class PIF
    {
        #region Public Properties

        public IEnumerable<Location> Locations { get; set; }

        public IEnumerable<NetworkLink> NetworkLinks { get; set; }

        public IEnumerable<TimingLink> TimingLinks { get; set; }

        #endregion Public Properties
    }
}