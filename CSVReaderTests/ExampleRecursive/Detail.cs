using CSVReader.Attributes;

namespace ExampleRecursive.PEX
{
    [Type("TDT")]
    public class Detail : Action
    {
        #region Public Properties

        [DelimitedField(2)]
        public int CeROrdinal { get; set; }

        [DelimitedField(4)]
        public string ContractCode { get; set; }

        [DelimitedField(12)]
        public bool DriverOnlyOperation { get; set; }

        [DelimitedField(3)]
        public int OriginJourneyLegOrdinal { get; set; }

        [DelimitedField(9)]
        public string RAGauge { get; set; }

        [DelimitedField(10)]
        public string RouteCode { get; set; }

        [DelimitedField(5)]
        public string ServiceCode { get; set; }

        [DelimitedField(8)]
        public string Speed { get; set; }

        [DelimitedField(6)]
        public string TractionType { get; set; }

        [DelimitedField(7)]
        public string TrailingLoad { get; set; }

        [DelimitedField(11)]
        public string UICIdentifier { get; set; }

        #endregion Public Properties
    }
}