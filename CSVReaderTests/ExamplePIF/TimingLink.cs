using CSVReader.Attributes;

namespace ExamplePIF
{
    [Type("TLK")]
    public class TimingLink
        : Base
    {
        #region Public Properties

        [Field(14)]
        public string Description { get; set; }

        [Field(3)]
        public string DestinationLocation { get; set; }

        [Field(12)]
        public string EndDate { get; set; }

        [Field(9)]
        public int EntrySpeed { get; set; }

        [Field(10)]
        public int ExitSpeed { get; set; }

        [Field(2)]
        public string OriginLocation { get; set; }

        [Field(8)]
        public string RAGauge { get; set; }

        [Field(4)]
        public string RunningLineCode { get; set; }

        [Field(13)]
        public string SectionRunningTime { get; set; }

        [Field(7)]
        public string Speed { get; set; }

        [Field(11)]
        public string StartDate { get; set; }

        [Field(5)]
        public string TractionType { get; set; }

        [Field(6)]
        public string TrailingLoad { get; set; }

        #endregion Public Properties
    }
}