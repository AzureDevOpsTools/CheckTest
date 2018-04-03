using System;
using System.IO;
using System.Linq;

namespace CheckTest
{
    public class VerifyTestCounts
    {
        public  int Check(string path, int minimum, bool verbose)
        {
            if (!Directory.Exists(path))
            {
                Console.Error.WriteLine($"Path for testresults, {path}, not found");
                return -2;
            }

            var trxFiles = Directory.GetFiles(path, "*.trx", SearchOption.AllDirectories);
            if (!trxFiles.Any())
            {
                Console.Error.WriteLine($"Didn't find any trx files in {path}");
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

            if (count < minimum)
            {
                Console.Error.WriteLine($"Too few tests found, found {count}, expected minimum {minimum} ");
                return -4;
            }

            Console.WriteLine($"Found {count} tests, which is more than expected minimum {minimum}");
            return 0;
        }
    }
}