using CSVReader;
using CSVReaderTests.ExampleLastInfinite;
using ExamplePIF;
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
            reader.InitializeByAttributes<RouteDefinition>(",");

            var result = reader.Get<RouteDefinition>(@"..\..\..\ExampleLastInfinite\_Example.csv").ToArray();

            Assert.IsTrue(result.Count() == 7);
            Assert.IsTrue(result.First().Anchors.Count() == 11);
        }

        [Test]
        public void TestPIFImport()
        {
            var reader = new Reader();
            reader.InitializeByAttributes<PIF>(",");

            var result = reader.Get<PIF>(@"..\..\..\ExamplePIF\_Example.txt").ToArray();

            Assert.IsTrue(result.First().Locations.Count() == 10969);
            Assert.IsTrue(result.First().NetworkLinks.Count() == 543);
            Assert.IsTrue(result.First().TimingLinks.Count() == 16153);
        }

        [Test]
        public void TestRecursive()
        {
            var reader = new Reader();
            reader.InitializeByAttributes<Offer>(",");

            var result = reader.Get<Offer>(@"..\..\..\ExampleRecursive\_Example.PEX").ToArray();

            Assert.IsTrue(result.Count() == 1);
            Assert.IsTrue(result.First().Trains.Count() == 5);
        }

        #endregion Public Methods
    }
}