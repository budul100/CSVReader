using CSVReader.Attributes;
using System;

namespace ExamplePIF
{
    [Type("NWK")]
    public class NetworkLink
        : Base
    {
        #region Public Properties

        [Field(3)]
        public string DestinationLocation { get; set; }

        [Field(10)]
        public int Distance { get; set; }

        [Field(12)]
        public char DOONonPassenger { get; set; }

        [Field(11)]
        public char DOOPassenger { get; set; }

        [Field(7)]
        public DateTime EndDate { get; set; }

        [Field(9)]
        public char FinalDirection { get; set; }

        [Field(8)]
        public char InitalDirection { get; set; }

        [Field(18)]
        public int MaximumTrainLength { get; set; }

        [Field(2)]
        public string OriginLocation { get; set; }

        [Field(16)]
        public string PowerSupplyType { get; set; }

        [Field(13)]
        public char RadioElectricTokenBlock { get; set; }

        [Field(15)]
        public char ReversibleLine { get; set; }

        [Field(17)]
        public string RouteAvailabilityNumber { get; set; }

        [Field(4)]
        public string RunningLineCode { get; set; }

        [Field(5)]
        public string RunningLineDescription { get; set; }

        [Field(6)]
        public DateTime StartDate { get; set; }

        [Field(14)]
        public string Zone { get; set; }

        #endregion Public Properties
    }
}