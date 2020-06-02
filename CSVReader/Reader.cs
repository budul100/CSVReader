using CSVReader.Attributes;
using CSVReader.Extensions;
using CSVReader.Factories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace CSVReader
{
    public class Reader
    {
        #region Private Fields

        private static readonly char[] lineSeparators = new[] { '\r', '\n' };

        private RecordFactory baseFactory;
        private Regex trimRegex;
        private char[] valueSeparators;

        #endregion Private Fields

        #region Public Methods

        public IEnumerable<object> Get(string path, IProgress<double> progress = default)
        {
            var pathes = new string[] { path };

            var result = Get(
                pathes: pathes,
                progress: progress);

            return result;
        }

        public IEnumerable<T> Get<T>(string path, IProgress<double> progress = default)
            where T : class
        {
            var contents = Get(
                path: path,
                progress: progress);

            foreach (var content in contents)
            {
                yield return content as T;
            }
        }

        public IEnumerable<T> Get<T>(IEnumerable<string> pathes, IProgress<double> progress = default)
            where T : class
        {
            var contents = Get(
                pathes: pathes,
                progress: progress);

            foreach (var content in contents)
            {
                yield return content as T;
            }
        }

        public IEnumerable<object> Get(IEnumerable<string> pathes, IProgress<double> progress = default)
        {
            if (baseFactory == default)
            {
                throw new ApplicationException("The reader must be initialized at first.");
            }

            var pathesIndex = 0;

            foreach (var path in pathes)
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"The file '{path}' does not exist.");
                }

                var progressSetter = GetProgessSetter(
                    progress: progress,
                    pathesIndex: pathesIndex,
                    pathesSum: pathes.Count());

                var lines = GetLines(
                    path: path,
                    trimRegex: trimRegex).ToArray();

                var linesIndex = 0;

                foreach (var line in lines)
                {
                    try
                    {
                        var contents = line
                            .Split(valueSeparators).ToArray();

                        baseFactory.SetContents(contents);
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException(
                            message: $"The file '{path}' cannot be red.",
                            innerException: ex);
                    }

                    if (baseFactory.IsNewRecord)
                        yield return baseFactory.Record;

                    progressSetter?.Invoke(
                        arg1: linesIndex,
                        arg2: lines.Count());
                }

                baseFactory.Dispose();

                pathesIndex++;
            }
        }

        public void InitializeByAttributes(Type type, string delimiters = ",", bool trimValues = true)
        {
            baseFactory = new RecordFactory(
                type: type,
                trimValues: trimValues);

            if (baseFactory != default)
            {
                baseFactory.InitializeByAttributes();
                baseFactory.CompleteInitialization();

                trimRegex = GetTrimRegex(type);
                valueSeparators = GetDelimiters(
                    type: type,
                    given: delimiters).ToArray();
            }
        }

        public void InitializeByAttributes<T>(string delimiters = ",", bool trimValues = true)
        {
            InitializeByAttributes(
                type: typeof(T),
                delimiters: delimiters,
                trimValues: trimValues);
        }

        #endregion Public Methods

        #region Private Methods

        private IEnumerable<char> GetDelimiters(Type type, string given)
        {
            var delimiters = type.GetAttribute<SetAttribute>()?.Delimiters
                ?? given;

            var result = delimiters?.ToCharArray()
                ?? Enumerable.Empty<char>();

            return result;
        }

        private IEnumerable<string> GetLines(string path, Regex trimRegex)
        {
            var text = File.ReadAllText(path);

            if (trimRegex?.IsMatch(text) ?? false)
            {
                text = trimRegex.Match(text).Value;
            }

            var result = text.Split(lineSeparators)
                .Where(t => !string.IsNullOrEmpty(t)).ToArray();

            return result;
        }

        private Action<int, int> GetProgessSetter(IProgress<double> progress, int pathesIndex, int pathesSum)
        {
            var pathesSumDouble = (double)pathesSum;
            var pathesProgess = pathesIndex / pathesSumDouble;

            return (linesIndex, linesSum) => progress?.Report(pathesProgess + (linesIndex++ / (pathesSumDouble * linesSum)));
        }

        private Regex GetTrimRegex(Type type)
        {
            var result = default(Regex);

            var fileAttribute = type.GetAttribute<SetAttribute>();

            if (fileAttribute != default
                && !string.IsNullOrWhiteSpace(fileAttribute.TrimRegex))
            {
                result = new Regex(
                    pattern: $"{fileAttribute.TrimRegex}",
                    options: RegexOptions.Singleline);
            }

            return result;
        }

        #endregion Private Methods
    }
}