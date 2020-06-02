using CSVReader.Attributes;

namespace ExampleRecursive.PEX
{
    [Type("TDT")]
    public class Detail : Action
    {
        #region Public Properties

        [Field(2)]
        public int CeROrdinal { get; set; }

        [Field(4)]
        public string ContractCode { get; set; }

        [Field(12)]
        public bool DriverOnlyOperation { get; set; }

        [Field(3)]
        public int OriginJourneyLegOrdinal { get; set; }

        [Field(9)]
        public string RAGauge { get; set; }

        [Field(10)]
        public string RouteCode { get; set; }

        [Field(5)]
        public string ServiceCode { get; set; }

        [Field(8)]
        public string Speed { get; set; }

        [Field(6)]
        public string TractionType { get; set; }

        [Field(7)]
        public string TrailingLoad { get; set; }

        [Field(11)]
        public string UICIdentifier { get; set; }

        #endregion Public Properties
    }
}