using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
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
            bool verbose = args.Length == 2 && args[2] == "-v";
            var path = args[0];
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Console.Error.WriteLine($"Path for testlog, {args[0]}, not found");
                return -2;
            }
            if (!File.Exists(path))
            {
                Console.Error.WriteLine($"TEstlog file not found, did you enable the /Diag option ?  ({path})");
                return -3;
            }

            var log = File.ReadAllLines(path);




            return 0;
        }
    }



    public class LogParser
    {
      
        readonly List<Exc> exceptions = new List<Exc>();
        public IEnumerable<Exc> Exceptions => exceptions;

        public LogParser(IEnumerable<string> log)
        {
            Exc exc = null;
            bool foundException = false;
            foreach (var line in log)
            {
                if (!foundException)
                {
                    if (line.StartsWith("V, ")) // Found a V-record
                    {
                        var elems = line.Split(',');
                        var msg = elems.FirstOrDefault(o => o.Trim().StartsWith("Message:"));
                        if (msg != null && msg.Contains("Exception:"))
                        {
                            foundException = true;
                            exc = new Exc(msg);
                        }
                    }
                }
                else
                {
                    if (line.StartsWith("V, ") || line.StartsWith("I, "))
                    {
                        foundException = false;
                        exceptions.Add(exc);
                    }
                }

            }
        }

    }


    public class Exc
    {
        public Exc(string msg)
        {
            var parts = msg.Split(':');
            if (parts.Length != 3)
            {
                Type = "Unknown";
                Message = msg;
            }
            else
            {
                Type = parts[1].Trim().TrimStart('\'');
                Message = parts[2].Trim();
            }
        }
        public string Type { get; set; }
        public string StackTrace { get; set; }
        public string Message { get; set; }
    }
}
