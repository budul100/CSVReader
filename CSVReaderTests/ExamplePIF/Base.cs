using CSVReader.Attributes;

namespace ExamplePIF
{
    public abstract class Base
    {
        #region Public Properties

        [Field(1)]
        public string ActionCode { get; set; }

        #endregion Public Properties
    }
}