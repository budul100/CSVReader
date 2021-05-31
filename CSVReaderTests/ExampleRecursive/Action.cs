using CSVReader.Attributes;

namespace ExampleRecursive.PEX
{
    public abstract class Action
    {
        #region Public Properties

        [DelimitedField(1)]
        public ActionCodes ActionCode { get; set; }

        #endregion Public Properties
    }
}