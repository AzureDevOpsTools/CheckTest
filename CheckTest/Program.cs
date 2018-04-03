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
            var checkTest = new VerifyTestCounts();
            return checkTest.Check(args[0],Convert.ToInt32(args[1]), verbose);
        }
    }


   

}
