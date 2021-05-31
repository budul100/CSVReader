using CSVReader.Attributes;
using System;
using System.Collections.Generic;

namespace ExampleRecursive.PEX
{
    [Type("THD")]
    public class Train : Action
    {
        #region Public Properties

        [DelimitedField(14)]
        public BidOfferStatus BidOfferStatus { get; set; }

        public IEnumerable<DateTime> Dates { get; set; }

        public int DatesIndex { get; set; }

        [DelimitedField(12)]
        public string DestinationLocation { get; set; }

        [DelimitedField(13)]
        public TimeSpan DestinationTime { get; set; }

        public Detail[] Details { get; set; }

        [DelimitedField(6)]
        public string InitialDateOfOperation { get; set; }

        public Movement[] Movements { get; set; }

        [DelimitedField(4)]
        public string NetworkRailTrainReference { get; set; }

        [DelimitedField(10)]
        public string OriginLocation { get; set; }

        [DelimitedField(11)]
        public TimeSpan OriginTime { get; set; }

        public Reference[] References { get; set; }

        public Stop[] Stops { get; set; }

        [DelimitedField(2)]
        public string TocId { get; set; }

        [DelimitedField(8)]
        public string TOCTrainIdentifier { get; set; }

        [DelimitedField(7)]
        public string TrainDates { get; set; }

        [DelimitedField(3)]
        public string TrainID { get; set; }

        [DelimitedField(5)]
        public string TrainLayer { get; set; }

        [DelimitedField(9)]
        public string TsdbUid { get; set; }

        #endregion Public Properties
    }
}