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
    public static class Reader<T>
        where T : class
    {
        #region Private Fields

        private static readonly char delimiter;
        private static readonly string[] newLines = new[] { "\r\n", "\r", "\n" };
        private static readonly Regex trimRegex;

        #endregion Private Fields

        #region Public Constructors

        static Reader()
        {
            trimRegex = GetTrimRegex();
            delimiter = GetDelimiter();
        }

        #endregion Public Constructors

        #region Public Methods

        public static IEnumerable<T> GetDatas(IEnumerable<string> pathes, IProgress<double> progress)
        {
            var pathesIndex = 0;
            var pathesSum = (double)pathes.Count();

            foreach (var path in pathes)
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"The file '{path}' does not exist.");
                }

                var result = default(T);

                try
                {
                    var lines = GetLines(path).ToArray();

                    var linesIndex = 0;
                    var linesSum = (double)lines.Count();

                    var deserializer = new ValueDeserializer(typeof(T));

                    foreach (var line in lines)
                    {
                        var values = line.Split(delimiter);
                        deserializer.Set(values);

                        progress?.Report((pathesIndex / pathesSum) + (linesIndex++ / (pathesSum * linesSum)));
                    }

                    result = deserializer.Get() as T;
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(
                        message: $"The file '{path}' cannot be red.",
                        innerException: ex);
                }

                yield return result;

                pathesIndex++;
            }
        }

        public static async Task<IEnumerable<T>> GetDatasAsync(IEnumerable<string> pathes, IProgress<double> progress)
        {
            var result = await Task.FromResult(GetDatas(
                pathes: pathes,
                progress: progress).ToArray());

            return result;
        }

        #endregion Public Methods

        #region Private Methods

        private static char GetDelimiter()
        {
            var attribute = typeof(T).GetAttribute<ImportFile>();

            return attribute?.Delimiter ?? default;
        }

        private static IEnumerable<string> GetLines(string path)
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

        #endregion Private Methods
    }
}