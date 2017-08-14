using System;
using System.IO;
using System.Linq;

namespace CheckTest
{
    class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length <2 || args.Length>3)
            {
                Console.Error.WriteLine(
                    "CheckTest PathToTestResultsFolder MinNumberOfTests [-v]  --- Check the trx files from PathToTestResultsFolder for number of tests (lines) >= MinNumberOfTests, option -v enables verbose messages");
                return -1;
            }
            bool verbose = args.Length == 3 && args[2] == "-v";
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
            if (verbose)
                Console.WriteLine($"Found {trxFiles.Length} trx files");
            foreach (var trxfile in trxFiles)
            {
                if (verbose)
                    Console.WriteLine($"Scanning file {trxfile}");
                var lines = File.ReadAllLines(trxfile);
                var contentline = lines.Single(o => o.Contains("<Counters"));
                var trxCounter = new TrxCounter(contentline);
                if (verbose)
                    Console.WriteLine($"Found {trxCounter.Executed} executed tests");
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
}
