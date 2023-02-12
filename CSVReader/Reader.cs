using CSVReader.Extensions;
using CSVReader.Factories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CSVReader
{
    public class Reader
    {
        #region Private Fields

        private static readonly char[] lineSeparators = new[] { '\r', '\n' };

        private readonly string delimiters;
        private readonly Encoding encoding;
        private readonly bool trimValues;

        private Func<RecordFactory> basefactoryGetter;
        private Regex trimRegex;

        #endregion Private Fields

        #region Public Constructors

        public Reader(Encoding encoding = default, string delimiters = default, bool trimValues = true)
        {
            this.delimiters = delimiters;
            this.trimValues = trimValues;

            this.encoding = encoding
                ?? Encoding.Default;
        }

        public Reader(Type type, Encoding encoding = default, string delimiters = default, bool trimValues = true)
            : this(encoding: encoding, delimiters: delimiters, trimValues: trimValues)
        {
            Initialize(type);
        }

        #endregion Public Constructors

        #region Public Methods

        public IEnumerable<object> Get(string path, IProgress<double> progress = default)
        {
            var pathes = new string[] { path };

            return GetRecords(
                paths: pathes,
                progress: progress).ToArray();
        }

        public IEnumerable<T> Get<T>(string path, IProgress<double> progress = default)
            where T : class
        {
            var records = Get(
                path: path,
                progress: progress).ToArray();

            foreach (var record in records)
            {
                yield return record as T;
            }
        }

        public IEnumerable<object> Get(IEnumerable<string> pathes, IProgress<double> progress = default)
        {
            return GetRecords(
                paths: pathes,
                progress: progress).ToArray();
        }

        public IEnumerable<T> Get<T>(IEnumerable<string> pathes, IProgress<double> progress = default)
            where T : class
        {
            var records = GetRecords(
                paths: pathes,
                progress: progress).ToArray();

            foreach (var record in records)
            {
                yield return record as T;
            }
        }

        public async Task<IEnumerable<T>> GetAsync<T>(string path, IProgress<double> progress = default)
            where T : class
        {
            return await Task.FromResult(Get<T>(
                path: path,
                progress: progress)).ConfigureAwait(false);
        }

        public async Task<IEnumerable<T>> GetAsync<T>(IEnumerable<string> pathes, IProgress<double> progress = default)
            where T : class
        {
            return await Task.FromResult(Get<T>(
                pathes: pathes,
                progress: progress).ToArray()).ConfigureAwait(false);
        }

        public async Task<IEnumerable<object>> GetAsync(string path, IProgress<double> progress = default)
        {
            return await Task.FromResult(Get(
                path: path,
                progress: progress)).ConfigureAwait(false);
        }

        public async Task<IEnumerable<object>> GetAsync(IEnumerable<string> pathes, IProgress<double> progress = default)
        {
            return await Task.FromResult(Get(
                pathes: pathes,
                progress: progress).ToArray()).ConfigureAwait(false);
        }

        public void Initialize<T>()
        {
            Initialize(typeof(T));
        }

        #endregion Public Methods

        #region Private Methods

        private RecordFactory GetBaseFactory(Type type)
        {
            var valueSeparators = type.GetSeparators(delimiters).ToArray();

            var result = new RecordFactory(
                type: type,
                trimValues: trimValues,
                valueSeparators: valueSeparators);

            if (result != default)
            {
                if (type.HasFixedFields())
                {
                    result.InitializeFixeds();
                }
                else
                {
                    result.InitializeDelimiteds();
                }
            }

            return result;
        }

        private IEnumerable<string> GetLines(string path, Regex trimRegex)
        {
            var text = File.ReadAllText(
                path: path,
                encoding: encoding);

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
            var result = default(Action<int, int>);

            if (progress != default)
            {
                var pathesSumDouble = (double)pathesSum;
                var pathesProgess = pathesIndex / pathesSumDouble;

                result = (int linesIndex, int linesSum) => progress.Report(
                    value: pathesProgess + (linesIndex++ / (pathesSumDouble * linesSum)));
            }

            return result;
        }

        private IEnumerable<object> GetRecords(IEnumerable<string> paths, IProgress<double> progress)
        {
            if (basefactoryGetter == default)
            {
                throw new ApplicationException("The reader must be initialized at first.");
            }

            if (paths?.Any() ?? false)
            {
                var pathesIndex = 0;

                foreach (var path in paths)
                {
                    if (!File.Exists(path))
                    {
                        throw new FileNotFoundException($"The file '{path}' does not exist.");
                    }

                    var progressSetter = GetProgessSetter(
                        progress: progress,
                        pathesIndex: pathesIndex,
                        pathesSum: paths.Count());

                    var lines = GetLines(
                        path: path,
                        trimRegex: trimRegex).ToArray();

                    using (var baseFactory = basefactoryGetter())
                    {
                        var linesIndex = 0;

                        foreach (var line in lines)
                        {
                            try
                            {
                                baseFactory.SetContents(line);
                            }
                            catch (Exception ex)
                            {
                                throw new ApplicationException(
                                    message: $"The file '{path}' cannot be read.",
                                    innerException: ex);
                            }

                            if (baseFactory.IsNewRecord)
                            {
                                yield return baseFactory.Record;
                            }

                            progressSetter?.Invoke(
                                arg1: linesIndex++,
                                arg2: lines.Length);
                        }
                    }

                    pathesIndex++;
                }
            }
        }

        private void Initialize(Type type)
        {
            basefactoryGetter = () => GetBaseFactory(type);
            trimRegex = type.GetTrimRegex();
        }

        #endregion Private Methods
    }
}