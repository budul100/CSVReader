using CSVReader.Deserializers;
using System;
using System.Text.RegularExpressions;

namespace CSVReader.Models
{
    internal class ChildDefinition
    {
        #region Public Properties

        public BaseDeserializer Deserializer { get; set; }

        public Regex HeaderRegex { get; set; }

        public Action<object> Setter { get; set; }

        #endregion Public Properties
    }
}