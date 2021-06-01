using CSVReader.Attributes;
using System;

namespace IRMA
{
    [Type(
        headerRegex: "OO")]
    public class Timings
    {
        #region Public Properties

        public TimeSpan OffTime { get; set; }

        [FixedField(
            start: 11,
            length: 4)]
        public string OffTimeText { get; set; }

        public TimeSpan OnTime { get; set; }

        [FixedField(
            start: 7,
            length: 4)]
        public string OnTimeText { get; set; }

        #endregion Public Properties
    }
}