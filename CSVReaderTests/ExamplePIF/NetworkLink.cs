using CSVReader.Attributes;
using System;

namespace ExamplePIF
{
    [Type("NWK")]
    public class NetworkLink
        : Base
    {
        #region Public Properties

        [DelimitedField(3)]
        public string DestinationLocation { get; set; }

        [DelimitedField(10)]
        public int Distance { get; set; }

        [DelimitedField(12)]
        public char DOONonPassenger { get; set; }

        [DelimitedField(11)]
        public char DOOPassenger { get; set; }

        [DelimitedField(7)]
        public DateTime EndDate { get; set; }

        [DelimitedField(9)]
        public char FinalDirection { get; set; }

        [DelimitedField(8)]
        public char InitalDirection { get; set; }

        [DelimitedField(18)]
        public int MaximumTrainLength { get; set; }

        [DelimitedField(2)]
        public string OriginLocation { get; set; }

        [DelimitedField(16)]
        public string PowerSupplyType { get; set; }

        [DelimitedField(13)]
        public char RadioElectricTokenBlock { get; set; }

        [DelimitedField(15)]
        public char ReversibleLine { get; set; }

        [DelimitedField(17)]
        public string RouteAvailabilityNumber { get; set; }

        [DelimitedField(4)]
        public string RunningLineCode { get; set; }

        [DelimitedField(5)]
        public string RunningLineDescription { get; set; }

        [DelimitedField(6)]
        public DateTime StartDate { get; set; }

        [DelimitedField(14)]
        public string Zone { get; set; }

        #endregion Public Properties
    }
}