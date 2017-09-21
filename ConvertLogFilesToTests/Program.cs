using System;
using ConvertLogFilesToTrx.Vstst;

namespace ConvertLogFilesToTrx
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 1 || args.Length > 3)
            {
                Console.Error.WriteLine(
                    "ConvertLogFilesToTrx logfilename [pattern.config.xml] [-v]  --- Convert logfiles to trx files.  One line per test,    -v enables verbose messages");
                return -1;
            }
            bool verbose = (args.Length == 3 && args[2] == "-v") || (args.Length == 2 && args[1] == "-v");

            var converter = args.Length == 1 || (args.Length == 2 && args[1] == "-v") 
                ? new Converter(args[0], verbose) 
                : new Converter(args[0], args[1], verbose);

            return 0;
        }



       


    }
}
