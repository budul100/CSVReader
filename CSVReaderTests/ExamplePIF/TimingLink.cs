using CSVReader.Attributes;

namespace ExamplePIF
{
    [Type("TLK")]
    public class TimingLink
        : Base
    {
        #region Public Properties

        [DelimitedField(14)]
        public string Description { get; set; }

        [DelimitedField(3)]
        public string DestinationLocation { get; set; }

        [DelimitedField(12)]
        public string EndDate { get; set; }

        [DelimitedField(9)]
        public int EntrySpeed { get; set; }

        [DelimitedField(10)]
        public int ExitSpeed { get; set; }

        [DelimitedField(2)]
        public string OriginLocation { get; set; }

        [DelimitedField(8)]
        public string RAGauge { get; set; }

        [DelimitedField(4)]
        public string RunningLineCode { get; set; }

        [DelimitedField(13)]
        public string SectionRunningTime { get; set; }

        [DelimitedField(7)]
        public string Speed { get; set; }

        [DelimitedField(11)]
        public string StartDate { get; set; }

        [DelimitedField(5)]
        public string TractionType { get; set; }

        [DelimitedField(6)]
        public string TrailingLoad { get; set; }

        #endregion Public Properties
    }
}