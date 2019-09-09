using System;

namespace CSVReader.Internals
{
    internal class ChildDeserializer
    {
        #region Public Properties

        public bool IsAlreadySet { get; set; }

        public bool IsEnumerable { get; set; }

        public IDeserializer Deserializer { get; set; }

        public Action ValueSetter { get; set; }

        #endregion Public Properties
    }
}