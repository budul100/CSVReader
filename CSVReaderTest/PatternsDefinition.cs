using CSVReader.Attributes;
using IVUData.Network;

namespace IVUNetworkConverter.Settings
{
    [ImportFile(delimiter: ";")]
    internal class PatternsDefinition
    {
        #region Public Properties

        public Pattern[] Patterns { get; set; }

        #endregion Public Properties
    }
}