using CSVReader.Attributes;

namespace ExampleRecursive.PEX
{
    [Type("TRF")]
    public class Reference : Action
    {
        #region Public Properties

        [Field(2)]
        public int DetailOrdinal { get; set; }

        [Field(3)]
        public int JourneyOrdinal { get; set; }

        [Field(5)]
        public string ReferenceCode { get; set; }

        [Field(4)]
        public ReferenceCodes ReferenceCodeType { get; set; }

        #endregion Public Properties
    }
}