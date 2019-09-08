using CSVReader.Attributes;
using CSVReader.Deserializers;
using CSVReader.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CSVReader
{
    public class Reader<T>
        where T : class
    {
        #region Private Fields

        private static readonly char delimiter;
        private static readonly Regex trimRegex;
        private readonly ValueDeserializer deserializer;
        private readonly string[] newLines = new[] { "\r\n", "\r", "\n" };

        #endregion Private Fields

        #region Public Constructors

        static Reader()
        {
            trimRegex = GetTrimRegex();
            delimiter = GetDelimiter();
        }

        public Reader()
        {
            deserializer = new ValueDeserializer(typeof(T));
        }

        #endregion Public Constructors

        #region Public Methods

        public T GetData(string path, IProgress<double> progress)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"The file '{path}' does not exist.");
            }

            try
            {
                var lines = GetLines(path).ToArray();

                var index = 0;
                var sum = (double)lines.Count();

                foreach (var line in lines)
                {
                    var values = line.Split(delimiter);
                    deserializer.Set(values);

                    progress?.Report(index++ / sum);
                }

                return deserializer.Content as T;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    message: $"The file '{path}' cannot be red.",
                    innerException: ex);
            }
        }

        public async Task<T> GetDataAsync(string path, IProgress<double> progress)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"The file '{path}' does not exist.");
            }

            var result = await Task.FromResult(GetData(
                path: path,
                progress: progress));

            return result;
        }

        #endregion Public Methods

        #region Private Methods

        private static char GetDelimiter()
        {
            var attribute = typeof(T).GetAttribute<ImportFile>();

            return attribute?.Delimiter ?? default;
        }

        private static Regex GetTrimRegex()
        {
            var result = default(Regex);

            var attribute = typeof(T).GetAttribute<ImportFile>();

            if (attribute != null)
            {
                result = !string.IsNullOrWhiteSpace(attribute.TrimRegex)
                    ? new Regex($"{attribute.TrimRegex}", RegexOptions.Singleline)
                    : null;
            }

            return result;
        }

        private IEnumerable<string> GetLines(string path)
        {
            var text = File.ReadAllText(path);

            if (trimRegex?.IsMatch(text) ?? false)
            {
                text = trimRegex.Match(text).Value;
            }

            var result = text.Split(
                separator: newLines,
                options: StringSplitOptions.None);

            return result;
        }

        #endregion Private Methods
    }
}