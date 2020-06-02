using CSVReader.Attributes;

namespace ExampleRecursive.PEX
{
    [Type("TSP")]
    public class Stop : JourneyLeg
    {
        #region Public Properties

        [Field(4)]
        public string Arrival { get; set; }

        [Field(7)]
        public string ArrivalAdvertised { get; set; }

        [Field(5)]
        public string Departure { get; set; }

        [Field(8)]
        public string DepartureAdvertised { get; set; }

        [Field(10)]
        public bool EndPivot { get; set; }

        [Field(3)]
        public string Location { get; set; }

        [Field(6)]
        public string Platform { get; set; }

        [Field(9)]
        public bool StartPivot { get; set; }

        #endregion Public Properties
    }
}