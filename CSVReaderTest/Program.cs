using CSVReader;
using ShellProgressBar;
using System;
using System.Diagnostics;

namespace CSVReaderTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var x = Reader<PEX.Offer>.GetData(@"D:\Users\mgr\Desktop\MTR\IVU2UK\LTP_2019-12\LTP DEC 19.pex", null);

            stopWatch.Stop();
        }
    }
}
