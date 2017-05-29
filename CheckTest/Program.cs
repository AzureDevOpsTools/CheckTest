using System;
using System.IO;
using System.Linq;

namespace CheckTest
{
    class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.Error.WriteLine(
                    "CheckTest PathToTestResultsFolder MinNumberOfTests   --- Check the trx files from PathToTestResultsFolder for number of tests (lines) >= MinNumberOfTests");
                return -1;
            }
            if (!Directory.Exists(args[0]))
            {
                Console.Error.WriteLine($"Path for testresults, {args[0]}, not found");
                return -2;
            }
            var trxFiles = Directory.GetFiles(args[0], "*.trx", SearchOption.AllDirectories);
            if (!trxFiles.Any())
            {
                Console.Error.WriteLine($"Didn't find any trx files in {args[0]}");
                return -3;
            }
            int count = 0;
            foreach (var trxfile in trxFiles)
            {
                var lines = File.ReadAllLines(trxfile);
                var contentline = lines.Single(o => o.Contains("<Counters"));
                var trxCounter = new TrxCounter(contentline);
                count += trxCounter.Executed;
            }
            int expectedMin = Convert.ToInt32(args[1]);
            if (count < expectedMin)
            {
                Console.Error.WriteLine($"Too few tests found, found {count}, expected minimum {expectedMin} ");
                return -4;
            }
            Console.WriteLine($"Found {count} tests, which is more than expected minimum {expectedMin}");
            return 0;

        }
    }

    public class TrxCounter
    {
        private readonly string content;
        public TrxCounter(string content)
        {
            this.content = content;
            Total = ExtractNumber("total");
            Executed = ExtractNumber("executed");
            Passed = ExtractNumber("passed");
            Failed = ExtractNumber("failed");
        }

        private  string ExtractNumberStr(string term)
        {
            int indexOfTerm = content.IndexOf(term+"=", StringComparison.InvariantCultureIgnoreCase);
            int lengthOfTerm = term.Length + 2;
            var startStr = content.Substring(indexOfTerm + lengthOfTerm);
            int indexOfEndOfTerm = startStr.IndexOf("\"", StringComparison.Ordinal);
            var termValueStr = content.Substring(indexOfTerm + lengthOfTerm, indexOfEndOfTerm);
            return termValueStr;
        }

        private int ExtractNumber(string term)
        {
            var str = ExtractNumberStr(term);
            return Convert.ToInt32(str);
        }

        public int Total { get; }
        public int Executed { get; }
        public int Passed { get; private set; }

        public int Failed { get; set; }
    }


}
