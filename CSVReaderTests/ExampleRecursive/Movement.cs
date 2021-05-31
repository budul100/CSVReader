using CSVReader.Attributes;
using System;

namespace ExampleRecursive.PEX
{
    [Type("TMV")]
    public class Movement : JourneyLeg
    {
        #region Public Properties

        [DelimitedField(13)]
        public string Adjustment { get; set; }

        [DelimitedField(4)]
        public string Destination { get; set; }

        [DelimitedField(10)]
        public string EngineeringAllowance { get; set; }

        [DelimitedField(8)]
        public int EntrySpeed { get; set; }

        [DelimitedField(9)]
        public int ExitSpeed { get; set; }

        [DelimitedField(3)]
        public string Origin { get; set; }

        [DelimitedField(11)]
        public string PathingAllowance { get; set; }

        [DelimitedField(12)]
        public string PerformanceAllowance { get; set; }

        [DelimitedField(5)]
        public string RunningLineCode { get; set; }

        [DelimitedField(7)]
        public TimeSpan? TimeEnd { get; set; }

        [DelimitedField(6)]
        public TimeSpan? TimeStart { get; set; }

        #endregion Public Properties
    }
}