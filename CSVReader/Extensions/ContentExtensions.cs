using CSVReader.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CSVReader.Extensions
{
    internal static class ContentExtensions
    {
        #region Public Methods

        public static IEnumerable<string> GetContents(this string line, IEnumerable<Func<string, string>> getters,
            bool trimValues)
        {
            if (getters?.Any() ?? false)
            {
                foreach (var getter in getters)
                {
                    var result = getter.Invoke(line);

                    if (trimValues)
                    {
                        result = result.Trim();
                    }

                    yield return result;
                }
            }
        }

        public static IEnumerable<string> GetContents(this string line, char[] separator, bool trimValues)
        {
            var result = line.Split(separator);

            if (trimValues)
            {
                result = result
                    .Select(v => v.Trim()).ToArray();
            }

            return result;
        }

        public static string GetFixedText(this string line, int start, int length)
        {
            var result = default(string);

            if (!string.IsNullOrWhiteSpace(line))
            {
                if (start >= line.Length)
                {
                    throw new FixedValueNotReadableException(
                        line: line,
                        start: start);
                }

                length = start + length > line.Length
                    ? line.Length - start
                    : length;

                result = line.Substring(
                    startIndex: start,
                    length: length);
            }

            return result;
        }

        public static string GetFrontPattern(this string pattern)
        {
            if (!string.IsNullOrWhiteSpace(pattern))
            {
                if (!pattern.StartsWith("^"))
                {
                    pattern = $"^{pattern}";
                }

                if (!pattern.EndsWith(".+"))
                {
                    pattern = $"{pattern}.+";
                }
            }

            return pattern;
        }

        #endregion Public Methods
    }
}