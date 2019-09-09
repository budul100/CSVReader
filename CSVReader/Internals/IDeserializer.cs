using System.Text.RegularExpressions;

namespace CSVReader.Internals
{
    public interface IDeserializer
    {
        #region Public Properties

        object Content { get; }

        Regex HeaderRegex { get; }

        #endregion Public Properties

        #region Public Methods

        void Initialize();

        bool Set(string[] values);

        void Terminate();

        #endregion Public Methods
    }
}