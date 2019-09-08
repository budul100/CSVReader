using System.Collections.Generic;

namespace CSVReader.Internals
{
    public interface IDeserializer
    {
        #region Public Properties

        object Content { get; }

        string Header { get; }

        #endregion Public Properties

        #region Public Methods

        void Initialize();

        bool Set(string[] values);

        #endregion Public Methods
    }
}