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

        private static readonly string[] newLines = new[] { "\r\n", "\r", "\n" };

        #endregion Private Fields

        #region Public Methods

        public static T Get(string path, IProgress<double> progress = default)
        {
            var pathes = new string[] { path };

            return Get(
                pathes: pathes,
                progress: progress).SingleOrDefault();
        }

        public static IEnumerable<T> Get(IEnumerable<string> pathes, IProgress<double> progress = default)
        {
            var pathesIndex = 0;
            var pathesSum = (double)pathes.Count();

            foreach (var path in pathes)
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"The file '{path}' does not exist.");
                }

                var result = Get(
                    path: path,
                    progress: progress,
                    pathesIndex: pathesIndex,
                    pathesSum: pathesSum);

                yield return result;

                pathesIndex++;
            }
        }

        public static async Task<T> GetAsync(string path, IProgress<double> progress = default)
        {
            var result = await Task.FromResult(Get(
                path: path,
                progress: progress));

            return result;
        }

        public static async Task<IEnumerable<T>> GetAsync(IEnumerable<string> pathes, IProgress<double> progress = default)
        {
            var result = await Task.FromResult(Get(
                pathes: pathes,
                progress: progress).ToArray());

            return result;
        }

        #endregion Public Methods

        #region Private Methods

        private static T Get(string path, IProgress<double> progress, int pathesIndex, double pathesSum)
        {
            try
            {
                var lines = GetLines(path).ToArray();

                var linesIndex = 0;
                var linesSum = (double)lines.Length;

                var delimiter = GetDelimiter().ToArray();

                var deserializer = new ValueDeserializer(typeof(T));

                foreach (var line in lines)
                {
                    var values = line.Split(delimiter);
                    deserializer.Set(values);

                    progress?.Report((pathesIndex / pathesSum) + (linesIndex++ / (pathesSum * linesSum)));
                }

                var result = deserializer.Get() as T;

                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    message: $"The file '{path}' cannot be red.",
                    innerException: ex);
            }
        }

        private static IEnumerable<char> GetDelimiter()
        {
            var attribute = typeof(T).GetAttribute<ImportFileAttribute>();

            return attribute?.Delimiter?.ToCharArray()
                ?? Enumerable.Empty<char>();
        }

        private static IEnumerable<string> GetLines(string path)
        {
            var text = File.ReadAllText(path);

            var trimRegex = GetTrimRegex();

            if (trimRegex?.IsMatch(text) ?? false)
            {
                text = trimRegex.Match(text).Value;
            }

            var result = text.Split(
                separator: newLines,
                options: StringSplitOptions.None)
                .Where(t => !string.IsNullOrEmpty(t)).ToArray();

            return result;
        }

        private static Regex GetTrimRegex()
        {
            var result = default(Regex);

            var attribute = typeof(T).GetAttribute<ImportFileAttribute>();

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