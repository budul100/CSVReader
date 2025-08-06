using System.Linq;
using CSVReader;
using CSVReader.Exceptions;
using CSVReaderTests.ExampleLastInfinite;
using ExamplePIF;
using ExampleRecursive.PEX;

namespace CSVReaderTests
{
    public class Tests
    {
        #region Public Methods

        [Fact]
        public void TestIRMAImport()
        {
            var reader = new Reader();
            reader.Initialize<IRMA.Set>();

            var result = reader.Get<IRMA.Set>(@"..\..\..\ExampleIRMA\_Example.txt").FirstOrDefault();

            Assert.Equal(487, result.Diagrams.Count());
            Assert.True(result.Diagrams.All(d => d.Details != default));
            Assert.Equal(7, result.Diagrams.First().Skills.Count());
        }

        [Fact]
        public void TestLastInfinite()
        {
            var reader = new Reader();
            reader.Initialize<RouteDefinition>();

            var result = reader.Get<RouteDefinition>(@"..\..\..\ExampleLastInfinite\_Example.csv").ToArray();

            Assert.Equal(7, result.Length);
            Assert.Equal(11, result[0].Anchors.Count());
        }

        [Fact]
        public void TestMultipleFiles()
        {
            var reader = new Reader();
            reader.Initialize<OfferWithHeader>();

            var pathes = new string[]
            {
                @"..\..\..\ExamplePEX\_Example.PEX",
                @"..\..\..\ExamplePEX\_Example.PEX"
            };
            var result = reader.Get<Offer>(pathes).ToArray();

            Assert.Equal(2, result.Length);
            Assert.Contains(result, r => r.Trains.Length == 5);
            Assert.Contains(result, r => r.TimetableStartDate != default);
        }

        [Fact]
        public void TestPIFImport()
        {
            var reader = new Reader();
            reader.Initialize<PIF>();

            var result = reader.Get<PIF>(@"..\..\..\ExamplePIF\_Example.txt").FirstOrDefault();

            Assert.Equal(10969, result.Locations.Count());
            Assert.Equal(543, result.NetworkLinks.Count());
            Assert.Equal(16153, result.TimingLinks.Count());
        }

        [Fact]
        public void TestRecursiveWithHeader()
        {
            var reader = new Reader();
            reader.Initialize<OfferWithHeader>();

            var result = reader.Get<Offer>(@"..\..\..\ExamplePEX\_Example.PEX").ToArray();

            Assert.True(result.Single().TimetableStartDate != default);
            Assert.Equal(5, result.Single().Trains.Length);
        }

        [Fact]
        public void TestRecursiveWithoutHeader()
        {
            var reader = new Reader();
            reader.Initialize<OfferWithoutHeader>();

            var result = reader.Get<Offer>(@"..\..\..\ExamplePEX\_Example.PEX").ToArray();

            Assert.True(result.Single().TimetableStartDate == default);
            Assert.Equal(5, result.Single().Trains.Length);
        }

        [Fact]
        public void TestSameIndex()
        {
            var reader = new Reader(
                delimiters: ",");
            reader.Initialize<ExampleSameIndex.RouteDefinition>();

            Assert.Throws<PropertyAlreadySetException>(
                () => reader.Get<ExampleSameIndex.RouteDefinition>(@"..\..\..\ExampleSameIndex\_Example.txt").ToArray());
        }

        #endregion Public Methods
    }
}