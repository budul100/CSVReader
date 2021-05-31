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
        public void TestIRMAImport()
        {
            var reader = new Reader();
            reader.Initialize<IRMA.Set>();

            var result = reader.Get<IRMA.Set>(@"..\..\..\ExampleIRMA\_Example.txt").FirstOrDefault();

            Assert.IsTrue(result.Diagrams.Any());
        }

        [Test]
        public void TestLastInfinite()
        {
            var reader = new Reader(",");
            reader.Initialize<RouteDefinition>();

            var result = reader.Get<RouteDefinition>(@"..\..\..\ExampleLastInfinite\_Example.csv").ToArray();

            Assert.IsTrue(result.Count() == 7);
            Assert.IsTrue(result.First().Anchors.Count() == 11);
        }

        [Test]
        public void TestPIFImport()
        {
            var reader = new Reader();
            reader.Initialize<PIF>();

            var result = reader.Get<PIF>(@"..\..\..\ExamplePIF\_Example.txt").FirstOrDefault();

            Assert.IsTrue(result.Locations.Count() == 10969);
            Assert.IsTrue(result.NetworkLinks.Count() == 543);
            Assert.IsTrue(result.TimingLinks.Count() == 16153);
        }

        [Test]
        public void TestRecursive()
        {
            var reader = new Reader(",");
            reader.Initialize<Offer>();

            var result1 = reader.Get<Offer>(@"..\..\..\ExampleRecursive\_Example.PEX").FirstOrDefault();
            var result2 = reader.Get<Offer>(@"..\..\..\ExampleRecursive\_Example.PEX").FirstOrDefault();

            Assert.IsTrue(result1.Trains.Count() == 5);
            Assert.IsTrue(result2.Trains.Count() == 5);
        }

        #endregion Public Methods
    }
}