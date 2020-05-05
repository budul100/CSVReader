using IVUData.Network;
using NUnit.Framework;
using System.Linq;

namespace CSVReaderTest
{
    public class Tests
    {
        #region Private Fields

        private const string PatternPath = @"..\..\..\Patterns\PatternDefinitions.csv";

        #endregion Private Fields

        #region Public Methods

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestArray()
        {
            var result = CSVReader.Reader<PatternsDefinition>.Get(PatternPath);

            Assert.AreEqual(
                expected: "TND",
                actual: result.Patterns.First().Anchors.Last());
        }

        #endregion Public Methods
    }
}