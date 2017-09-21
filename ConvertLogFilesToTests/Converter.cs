using System;
using System.Collections.Generic;
using System.Linq;
using ConvertLogFilesToTests;
using WrapThat.SystemIO;

namespace ConvertLogFilesToTrx
{
    public class Pattern
    {
        public string Begin { get; set; } = "::BEGIN:";//Followed by name of test, ends with ::
        public string End { get; set; } = "::END::";

        public string SuccessOutcome { get; set; } = "::Success";
        public string FailedOutcome { get; set; } = "::Failed";

        public int MaxLines { get; set; } = 100;
    }

    public class Converter
    {
        const string ns = "microsoft.com/schemas/VisualStudio/TeamTest/2010";
        private string logfilename;
        private string outputfilename;
        private Pattern pattern;
        private bool verbose;
        private string v;

        public Converter(string v, bool verbose)
        {
            this.logfilename = v;
            this.verbose = verbose;
            outputfilename = System.IO.Path.ChangeExtension(v, "trx");
            pattern = new Pattern();
        }

        public Converter(string input, string pattern, bool verbose) : this(input,verbose)
        {
            this.pattern = Serializer.DeserializeObject<Pattern>(pattern);
        }

        private IFile File { get; set; }
        public void Execute(IFile file=null)
        {
            File = file ?? new File();
            var txt = File.ReadAllText(logfilename);

            var txtblocks = ExtractBlocks(txt);
            var testsStrings = ExtractTestStrings(txtblocks);
            var testrun = CreateTests(testsStrings);
            var xml = testrun.SerializeObjectWithOutNamespace(ns);
            File.WriteAllText(outputfilename,xml);
        }

        public TestRun CreateTests(IEnumerable<(string Name, string Msg, string Outcome)> testsStrings)
        {
            var  tr = new TestRun(logfilename,Environment.MachineName);
            foreach (var test in testsStrings)
            {
                var pass = test.Outcome == pattern.SuccessOutcome;
                tr.AddTestResult(test.Name,pass,new TimeInfo(DateTime.Now, DateTime.Now),test.Msg,"");
            }
            return tr;
        }

        public IEnumerable<(string Name,string Msg,string Outcome)> ExtractTestStrings(IEnumerable<string> txtblocks)
        {
            var list = new List<(string,string,string)>();

            foreach (var block in txtblocks.Where(o=>o.Length>0 && o[0]==':'))
            {
                var endOfName = block.IndexOf("::", StringComparison.InvariantCulture);
                var name = block.Substring(1, endOfName - 1);
                var rest = block.Substring(endOfName + 2);
                var posOutCome = rest.IndexOf("::",StringComparison.InvariantCulture);
                var outcome = rest.Substring(posOutCome + 2);
                rest = rest.Substring(0, posOutCome);
                list.Add((name,rest,outcome));
            }
            return list;

        }

        public IEnumerable<string> ExtractBlocks(string txt)
        {
            var splits = new[] {pattern.Begin, pattern.End,};
            var split = txt.Split(splits,StringSplitOptions.None);
            return new List<string>(split).Where(o=>o.StartsWith(":"));
        }
    }
}