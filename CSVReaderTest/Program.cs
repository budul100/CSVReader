using CSVReader;
using ShellProgressBar;
using System;

namespace CSVReaderTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var reader = new Reader<PEX.Offer>();
            var x = reader.GetData(@"D:\Users\mgr\HiDrive\Entwicklung\NET\Serializers\CSVReader\PEX\Examples\Export_2018-12-LTP.PEX", null);
        }
    }
}
