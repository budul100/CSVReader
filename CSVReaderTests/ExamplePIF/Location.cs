using CSVReader.Attributes;

namespace ExamplePIF
{
    [Type("LOC")]
    public class Location
        : Base
    {
        #region Public Properties

        [Field(6)]
        public int Easting { get; set; }

        [Field(5)]
        public string EndDate { get; set; }

        [Field(12)]
        public char ForceLBP { get; set; }

        [Field(2)]
        public string LocationCode { get; set; }

        [Field(3)]
        public string LocationName { get; set; }

        [Field(7)]
        public int Northing { get; set; }

        [Field(11)]
        public char OffNetwork { get; set; }

        [Field(10)]
        public int STANOX { get; set; }

        [Field(4)]
        public string StartDate { get; set; }

        [Field(8)]
        public char TimingPointType { get; set; }

        [Field(9)]
        public string Zone { get; set; }

        #endregion Public Properties
    }
}