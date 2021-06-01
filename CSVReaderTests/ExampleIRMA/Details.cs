using CSVReader.Attributes;
using System;

namespace IRMA
{
    [Type(
        headerRegex: "DD")]
    public class Details
    {
        #region Public Properties

        [FixedField(
            start: 18,
            length: 7)]
        public string Bitmask { get; set; }

        [FixedField(
            start: 10,
            length: 8)]
        public DateTime EndDate { get; set; }

        [FixedField(
            start: 2,
            length: 8)]
        public DateTime StartDate { get; set; }

        #endregion Public Properties
    }
}