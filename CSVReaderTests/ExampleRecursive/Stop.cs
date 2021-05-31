using CSVReader.Attributes;

namespace ExampleRecursive.PEX
{
    [Type("TSP")]
    public class Stop : JourneyLeg
    {
        #region Public Properties

        [DelimitedField(4)]
        public string Arrival { get; set; }

        [DelimitedField(7)]
        public string ArrivalAdvertised { get; set; }

        [DelimitedField(5)]
        public string Departure { get; set; }

        [DelimitedField(8)]
        public string DepartureAdvertised { get; set; }

        [DelimitedField(10)]
        public bool EndPivot { get; set; }

        [DelimitedField(3)]
        public string Location { get; set; }

        [DelimitedField(6)]
        public string Platform { get; set; }

        [DelimitedField(9)]
        public bool StartPivot { get; set; }

        #endregion Public Properties
    }
}