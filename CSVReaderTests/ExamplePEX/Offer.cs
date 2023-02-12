using CSVReader.Attributes;

namespace ExampleRecursive.PEX
{
    public class Offer
    {
        #region Public Properties

        [DelimitedField(7)]
        public string CycleStage { get; set; }

        [DelimitedField(6)]
        public string CycleType { get; set; }

        [DelimitedField(8)]
        public string FileCreationDate { get; set; }

        [DelimitedField(9)]
        public int FileSequenceNumber { get; set; }

        [DelimitedField(1)]
        public int FileVersion { get; set; }

        [DelimitedField(2)]
        public string SourceSystem { get; set; }

        [DelimitedField(5)]
        public string TimetableEndDate { get; set; }

        [DelimitedField(4)]
        public string TimetableStartDate { get; set; }

        [DelimitedField(3)]
        public string TocId { get; set; }

        public Train[] Trains { get; set; }

        #endregion Public Properties
    }
}