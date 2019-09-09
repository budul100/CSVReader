using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CSVReader.Deserializers
{
    internal interface IDeserializer
    {
        #region Public Properties

        Regex HeaderRegex { get; }

        #endregion Public Properties

        #region Public Methods

        object Get();

        void Set(IEnumerable<string> values);

        #endregion Public Methods
    }
}