using CSVReader.Attributes;
using System.Collections.Generic;

namespace IRMA
{
    [FixedSet(
        headerRegex: "HD")]
    public class Set
    {
        #region Public Properties

        public IEnumerable<Header> Diagrams { get; set; }

        #endregion Public Properties
    }
}