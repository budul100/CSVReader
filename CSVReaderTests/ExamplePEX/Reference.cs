using CSVReader.Attributes;

namespace ExampleRecursive.PEX
{
    [Type("TRF")]
    public class Reference : Action
    {
        #region Public Properties

        [DelimitedField(2)]
        public int DetailOrdinal { get; set; }

        [DelimitedField(3)]
        public int JourneyOrdinal { get; set; }

        [DelimitedField(5)]
        public string ReferenceCode { get; set; }

        [DelimitedField(4)]
        public ReferenceCodes ReferenceCodeType { get; set; }

        #endregion Public Properties
    }
}