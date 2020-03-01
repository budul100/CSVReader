using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CSVReader.Deserializers
{
    internal abstract class BaseDeserializer
    {
        #region Public Properties

        public Regex HeaderRegex { get; protected set; }

        #endregion Public Properties

        #region Public Methods

        public abstract object Get();

        public abstract void Set(IEnumerable<string> values);

        #endregion Public Methods
    }
}