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
            reader.Initialize<RouteDefinition>(",");

            var result = reader.Get<RouteDefinition>(@"..\..\..\ExampleLastInfinite\_Example.csv").ToArray();

            Assert.IsTrue(result.Count() == 7);
            Assert.IsTrue(result.First().Anchors.Count() == 11);
        }

        [Test]
        public void TestRecursive()
        {
            var reader = new Reader();
            reader.Initialize<Offer>(",");

            var result = reader.Get<Offer>(@"..\..\..\ExampleRecursive\_Example.PEX").ToArray();

            Assert.IsTrue(result.Count() == 1);
            Assert.IsTrue(result.First().Trains.Count() == 5);
        }

        #endregion Public Methods
    }
}