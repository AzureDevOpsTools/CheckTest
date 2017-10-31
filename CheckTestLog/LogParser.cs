using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheckTestLog.Test
{
    public class LogParser
    {

        readonly List<Exc> exceptions = new List<Exc>();
        public IEnumerable<Exc> Exceptions => exceptions;

        public LogParser(IEnumerable<string> log)
        {
            Exc exc = null;
            bool foundException = false;
            var builder = new StringBuilder();
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
                            if (elems.Length >= 7)
                                exc.Test = elems[7].Substring(0,elems[7].IndexOf(":")).Trim();

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
                    else
                    {
                        builder.Append(line + "\r");
                    }
                }

            }
            exc.StackTrace = builder.ToString();
        }

    }
}