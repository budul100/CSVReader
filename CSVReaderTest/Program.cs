using IVUData.Network;

namespace CSVReaderTest
{
    internal class Program
    {
        #region Private Methods

        private static void Main(string[] args)
        {
            var test = CSVReader.Reader<PatternsDefinition>.Get(@"..\..\..\PatternDefinitions.csv");
        }

        #endregion Private Methods
    }
}