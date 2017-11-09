using System;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CheckTestLog.Test
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 1 || args.Length > 2)
            {
                Console.Error.WriteLine(
                    "CheckTest PathToTestLogFile  Check the testlog file from diagnostics log for exceptions");
                return -1;
            }
            bool verbose = args.Length == 2 && args[1] == "-v";
            var path = args[0];
            if (verbose)
            {
                var currentdir = Directory.GetCurrentDirectory();
                Console.WriteLine($"Current directory is: {currentdir}");
            }

            if (!File.Exists(path))
            {
                Console.Error.WriteLine($"Testlog file not found, did you enable the /Diag option ?  ({path})");
                return -3;
            }

            var log = File.ReadAllLines(path);

            var parser = new LogParser(log);

            if (parser.Exceptions.Any())
            {
                foreach (var ex in parser.Exceptions)
                {
                    Console.Error.WriteLine($"Exception in test {ex.Test}, Type: {ex.Type},  Message: {ex.Message}");
                    if (verbose)
                        Console.Error.WriteLine($"Stacktrace: {ex.StackTrace}");
                }
                return parser.Exceptions.Count();
            }
            return 0;
        }
    }
}

