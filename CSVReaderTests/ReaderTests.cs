using CSVReader;
using CSVReaderTests.ExampleLastInfinite;
using ExampleRecursive.PEX;
using NUnit.Framework;
using System.Linq;

namespace CSVReaderTests
{
    public class Tests
    {
        #region Public Methods

        [Test]
        public void TestLastInfinite()
        {
            var reader = new Reader();
            reader.Initialize<PatternDefinition>(",");

            var result = reader.Get(@"..\..\..\ExampleLastInfinite\_Example.csv").ToArray();

            Assert.IsTrue(result.Count() == 7);
        }

        [Test]
        public void TestRecursive()
        {
            var reader = new Reader();
            reader.Initialize<Offer>(",");

            var result = reader.Get(@"..\..\..\ExampleRecursive\_Example.PEX").ToArray();

            Assert.IsTrue(result.Count() == 1);

            var trains = (result.First() as Offer).Trains;

            Assert.IsTrue(trains.Count() == 5);
        }

        #endregion Public Methods
    }
}