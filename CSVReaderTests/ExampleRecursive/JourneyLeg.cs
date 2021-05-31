using CSVReader.Attributes;

namespace ExampleRecursive.PEX
{
    public abstract class JourneyLeg : Action
    {
        #region Public Properties

        [DelimitedField(2)]
        public int JourneyLegOrdinal { get; set; }

        #endregion Public Properties
    }
}