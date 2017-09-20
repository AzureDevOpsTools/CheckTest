using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConvertLogFilesToTrx.Vstst;

namespace ConvertLogFilesToTrx
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 2 || args.Length > 3)
            {
                Console.Error.WriteLine(
                    "ConvertLogFilesToTrx PathToTestResultsFolder MinNumberOfTests [-v]  --- Check the trx files from PathToTestResultsFolder for number of tests (lines) >= MinNumberOfTests, option -v enables verbose messages");
                return -1;
            }
            bool verbose = args.Length == 3 && args[2] == "-v";
            return 0;
        }



        TestRunType CreateTestRun()
        {
            var tr = new TestRunType
            {
                id = new Guid().ToString(),
                name = "whatever",
                runUser = "whoever",
            };
            return tr;
        }


    }

    

    }
