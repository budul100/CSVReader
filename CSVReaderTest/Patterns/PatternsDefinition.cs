using CSVReader.Attributes;

namespace IVUData.Network
{
    [ImportFile(delimiter: ";")]
    internal class PatternsDefinition
    {
        #region Public Properties

        public Pattern[] Patterns { get; set; }

        #endregion Public Properties
    }
}