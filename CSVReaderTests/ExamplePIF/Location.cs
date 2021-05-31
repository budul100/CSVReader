using CSVReader.Attributes;

namespace ExamplePIF
{
    [Type("LOC")]
    public class Location
        : Base
    {
        #region Public Properties

        [DelimitedField(6)]
        public int Easting { get; set; }

        [DelimitedField(5)]
        public string EndDate { get; set; }

        [DelimitedField(12)]
        public char ForceLBP { get; set; }

        [DelimitedField(2)]
        public string LocationCode { get; set; }

        [DelimitedField(3)]
        public string LocationName { get; set; }

        [DelimitedField(7)]
        public int Northing { get; set; }

        [DelimitedField(11)]
        public char OffNetwork { get; set; }

        [DelimitedField(10)]
        public int STANOX { get; set; }

        [DelimitedField(4)]
        public string StartDate { get; set; }

        [DelimitedField(8)]
        public char TimingPointType { get; set; }

        [DelimitedField(9)]
        public string Zone { get; set; }

        #endregion Public Properties
    }
}