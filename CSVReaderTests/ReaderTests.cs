using CSVReader;
using CSVReader.Exceptions;
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

            Assert.IsTrue(result.Diagrams.Count() == 487);
            Assert.IsTrue(result.Diagrams.All(d => d.Details != default));

            Assert.IsTrue(result.Diagrams.First().Skills.Count() == 7);
        }

        [Test]
        public void TestLastInfinite()
        {
            var reader = new Reader();
            reader.Initialize<RouteDefinition>();

            var result = reader.Get<RouteDefinition>(@"..\..\..\ExampleLastInfinite\_Example.csv").ToArray();

            Assert.IsTrue(result.Length == 7);
            Assert.IsTrue(result[0].Anchors.Count() == 11);
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
        public void TestRecursiveWithHeader()
        {
            var reader = new Reader();
            reader.Initialize<OfferWithHeader>();

            var result = reader.Get<Offer>(@"..\..\..\ExampleRecursive\_Example.PEX").ToArray();

            Assert.IsTrue(result.Single().TimetableStartDate != default);
            Assert.IsTrue(result.Any(r => r.Trains.Length > 0));
            Assert.IsTrue(result[0].Trains.Length == 5);
        }

        [Test]
        public void TestRecursiveWithoutHeader()
        {
            var reader = new Reader();
            reader.Initialize<OfferWithoutHeader>();

            var result = reader.Get<Offer>(@"..\..\..\ExampleRecursive\_Example.PEX").ToArray();

            Assert.IsTrue(result.Single().TimetableStartDate == default);
            Assert.IsTrue(result.Any(r => r.Trains.Length > 0));
            Assert.IsTrue(result[0].Trains.Length == 5);
        }

        [Test]
        public void TestSameIndex()
        {
            var reader = new Reader(",");
            reader.Initialize<ExampleSameIndex.RouteDefinition>();

            Assert.Throws<PropertyAlreadySetException>(
                () => reader.Get<ExampleSameIndex.RouteDefinition>(@"..\..\..\ExampleSameIndex\_Example.txt").ToArray());
        }

        #endregion Public Methods
    }
}