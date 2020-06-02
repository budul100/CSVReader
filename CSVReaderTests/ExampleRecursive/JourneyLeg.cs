using CSVReader.Attributes;

namespace ExampleRecursive.PEX
{
    public abstract class JourneyLeg : Action
    {
        #region Public Properties

        [Field(2)]
        public int JourneyLegOrdinal { get; set; }

        #endregion Public Properties
    }
}