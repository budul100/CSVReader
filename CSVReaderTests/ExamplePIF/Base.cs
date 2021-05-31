using CSVReader.Attributes;

namespace ExamplePIF
{
    public abstract class Base
    {
        #region Public Properties

        [DelimitedField(1)]
        public string ActionCode { get; set; }

        #endregion Public Properties
    }
}