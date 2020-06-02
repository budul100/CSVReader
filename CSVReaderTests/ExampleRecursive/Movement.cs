using CSVReader.Attributes;
using System;

namespace ExampleRecursive.PEX
{
    [Type("TMV")]
    public class Movement : JourneyLeg
    {
        #region Public Properties

        [Field(13)]
        public string Adjustment { get; set; }

        [Field(4)]
        public string Destination { get; set; }

        [Field(10)]
        public string EngineeringAllowance { get; set; }

        [Field(8)]
        public int EntrySpeed { get; set; }

        [Field(9)]
        public int ExitSpeed { get; set; }

        [Field(3)]
        public string Origin { get; set; }

        [Field(11)]
        public string PathingAllowance { get; set; }

        [Field(12)]
        public string PerformanceAllowance { get; set; }

        [Field(5)]
        public string RunningLineCode { get; set; }

        [Field(7)]
        public TimeSpan? TimeEnd { get; set; }

        [Field(6)]
        public TimeSpan? TimeStart { get; set; }

        #endregion Public Properties
    }
}