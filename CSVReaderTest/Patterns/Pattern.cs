using CSVReader.Attributes;
using System.Collections.Generic;

namespace IVUData.Network
{
    [ImportRecord()]
    public class Pattern
    {
        #region Public Properties

        [ImportField(0)]
        public string Abbreviation { get; set; }

        [ImportField(3, 50)]
        public IList<string> Anchors { get; set; }

        [ImportField(1)]
        public string Description { get; set; }

        [ImportField(2)]
        public int Index { get; set; }

        public bool? IsBackward { get; set; }

        public bool IsDefinition { get; set; }

        public int Length { get; set; }

        #endregion Public Properties
    }
}