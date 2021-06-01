using CSVReader.Attributes;
using System;

namespace IRMA
{
    [Type(
        headerRegex: "DH")]
    public class Header
    {
        #region Public Properties

        [FixedField(
            start: 26,
            length: 2)]
        public string DepotCode { get; set; }

        public Details Details { get; set; }

        [FixedField(
            start: 28,
            length: 4)]
        public int? DiagramNumber { get; set; }

        [FixedField(
            start: 47,
            length: 5)]
        public string GradeCode { get; set; }

        public bool IsCancelled { get; set; }

        [FixedField(
            start: 52,
            length: 1)]
        public string IsCancelledMarker { get; set; }

        public bool IsLTP { get; set; }

        [FixedField(
            start: 39,
            length: 3)]
        public string IsLTPMarker { get; set; }

        public TimeSpan OffTime { get; set; }

        public TimeSpan OnTime { get; set; }

        #endregion Public Properties
    }
}