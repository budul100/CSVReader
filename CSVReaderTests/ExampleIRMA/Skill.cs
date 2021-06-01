using CSVReader.Attributes;

namespace IRMA
{
    [Type(
        headerPattern: "LX")]
    public class Skill
    {
        #region Public Properties

        [FixedField(
            start: 3,
            length: 3)]
        public int Code { get; set; }

        [FixedField(
            start: 2,
            length: 1)]
        public char Type { get; set; }

        #endregion Public Properties
    }
}