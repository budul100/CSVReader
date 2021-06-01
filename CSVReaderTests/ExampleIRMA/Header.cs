using CSVReader.Attributes;
using System;
using System.Collections.Generic;

namespace IRMA
{
    [Type(
        headerPattern: "DH")]
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
        public string IsCancelledText { get; set; }

        public bool IsLTP { get; set; }

        [FixedField(
            start: 39,
            length: 3)]
        public string IsLTPText { get; set; }

        public IEnumerable<Skill> Skills { get; set; }

        public Timings Timings { get; set; }

        #endregion Public Properties
    }
}