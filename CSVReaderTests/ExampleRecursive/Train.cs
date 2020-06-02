using CSVReader.Attributes;
using System;
using System.Collections.Generic;

namespace ExampleRecursive.PEX
{
    [Type("THD")]
    public class Train : Action
    {
        #region Public Properties

        [Field(14)]
        public BidOfferStatus BidOfferStatus { get; set; }

        public IEnumerable<DateTime> Dates { get; set; }

        public int DatesIndex { get; set; }

        [Field(12)]
        public string DestinationLocation { get; set; }

        [Field(13)]
        public TimeSpan DestinationTime { get; set; }

        public Detail[] Details { get; set; }

        [Field(6)]
        public string InitialDateOfOperation { get; set; }

        public Movement[] Movements { get; set; }

        [Field(4)]
        public string NetworkRailTrainReference { get; set; }

        [Field(10)]
        public string OriginLocation { get; set; }

        [Field(11)]
        public TimeSpan OriginTime { get; set; }

        public Reference[] References { get; set; }

        public Stop[] Stops { get; set; }

        [Field(2)]
        public string TocId { get; set; }

        [Field(8)]
        public string TOCTrainIdentifier { get; set; }

        [Field(7)]
        public string TrainDates { get; set; }

        [Field(3)]
        public string TrainID { get; set; }

        [Field(5)]
        public string TrainLayer { get; set; }

        [Field(9)]
        public string TsdbUid { get; set; }

        #endregion Public Properties
    }
}