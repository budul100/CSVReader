namespace CSVReader.Attributes
{
    public class ImportFile : ImportRecord
    {
        #region Public Constructors

        public ImportFile
            (string headerRegex, string trimRegex, char delimiter = '\t') : base(headerRegex)
        {
            TrimRegex = trimRegex;
            Delimiter = delimiter;
        }

        #endregion Public Constructors

        #region Public Properties

        public char Delimiter { get; }

        public string TrimRegex { get; }

        #endregion Public Properties
    }
}