using CSVReader.Attributes;

namespace ExampleRecursive.PEX
{
    [Set(
        trimRegex: ".*(?=PIT)",
        headerRegex: "PEX|PIF",
        delimiters: "\t")]
    public class Offer
    {
        #region Public Properties

        [Field(7)]
        public string CycleStage { get; set; }

        [Field(6)]
        public string CycleType { get; set; }

        [Field(8)]
        public string FileCreationDate { get; set; }

        [Field(9)]
        public int FileSequenceNumber { get; set; }

        [Field(1)]
        public int FileVersion { get; set; }

        [Field(2)]
        public string SourceSystem { get; set; }

        [Field(5)]
        public string TimetableEndDate { get; set; }

        [Field(4)]
        public string TimetableStartDate { get; set; }

        [Field(3)]
        public string TocId { get; set; }

        public Train[] Trains { get; set; }

        #endregion Public Properties
    }
}