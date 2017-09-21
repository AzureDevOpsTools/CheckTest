using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace ConvertLogFilesToTests
{


    public interface ITest
    {
        string Name { get; }
        bool Passed { get; }
        TimeInfo TimeInfo { get; }
        string Output { get; }
        string Error { get; }

    }



    // See https://blogs.msdn.microsoft.com/dhopton/2008/06/13/helpful-internals-of-trx-and-vsmdi-files/ for info


        [XmlRoot(nameof(TestRun),Namespace = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010")]
    public class TestRun
    {
        private readonly string computername;
        public Times Times { get; set; } = new Times();
        public TestSettings TestSettings { get; set; } = new TestSettings();

        public Results Results { get; set; } = new Results();

        public TestDefinitions TestDefinitions { get; set; } = new TestDefinitions();
        public TestEntries TestEntries { get; set; } = new TestEntries();
        public TestLists TestLists { get; set; } = new TestLists();
        public ResultSummary ResultSummary { get; set; } = new ResultSummary();

        [XmlAttribute("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("runUser")]
        public string RunUser { get; set; }

        //[XmlAttribute("xmlns")]
        //public string XmlNs { get; set; } = "http://microsoft.com/schemas/VisualStudio/TeamTest/2010";


        public TestRun()
        {
            this.namespaces = new XmlSerializerNamespaces(new XmlQualifiedName[] {
               new XmlQualifiedName(string.Empty, "http://microsoft.com/schemas/VisualStudio/TeamTest/2010") // Default Namespace
                // Add any other namespaces, with prefixes, here.
            });
        }

        public TestRun(string fileName, string computername) : this()
        {
            this.computername = computername;
            TestSettings.Deployment = new Deployment(fileName);
        }




        public void AddTestResult(string testname, bool passed, TimeInfo timeInfo, string output, string error)
        {
            var testlistid = TestLists.GuidOfResultsNotInList.ToString();
            var testResult = new TestResult(testname, computername, passed, timeInfo, output, error,testlistid);
            Results.UnitTestResults.Add(testResult.UnitTestResult);
            ResultSummary.IncCounters(passed);
            TestDefinitions.UnitTests.Add(testResult.UnitTest);
            TestEntries.TestEntryList.Add(testResult.TestEntry);
        }

        [XmlNamespaceDeclarations]
        public XmlSerializerNamespaces Namespaces => this.namespaces;

        private XmlSerializerNamespaces namespaces;

    }

    /// <summary>
    /// Used as a wrapper class for distribution of results to lower serializable classes
    /// </summary>
    public class TestResult
    {
        public UnitTestResult UnitTestResult { get; set; }
        public UnitTest UnitTest { get; set; }

        public TestEntry TestEntry { get; set; }
        public TestResult(string testname, string computername, bool passed, TimeInfo timeInfo, string output, string error,string testListId)
        {
            var utr = new UnitTestResult(testname, passed, timeInfo, output, error,testListId) {ComputerName = computername};
            var ut = new UnitTest(testname, utr.ExecutionId, utr.TestId);
            var te = new TestEntry(utr.TestId,utr.ExecutionId,testListId);
            UnitTestResult = utr;
            UnitTest = ut;
            TestEntry = te;
        }
    }

    /// <summary>
    /// Wrapper class for timeinfo
    /// </summary>
    public class TimeInfo
    {
        public DateTime Start { get; }
        public DateTime End { get; }
        public TimeSpan Duration { get; }

        public TimeInfo(DateTime start, TimeSpan duration)
        {
            Start = start;
            Duration = duration;
            End = Start + duration;
        }

        public TimeInfo(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
            Duration = End - Start;
        }

    }


    public class TestSettings
    {
        [XmlAttribute("name")]
        public string Name { get; set; } = "default";
        [XmlAttribute("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public Execution Execution { get; set; } = new Execution();

        public Deployment Deployment { get; set; } = new Deployment();

    }

    public class Times
    {

        [XmlAttribute("creation")]
        public DateTime Creation { get; set; }

        [XmlAttribute("queuing")]
        public DateTime Queuing { get; set; }

        [XmlAttribute("start")]
        public DateTime Start { get; set; }

        [XmlAttribute("finish")]
        public DateTime Finish { get; set; }

        public Times()
        {
            Creation = DateTime.Now;
            Queuing = DateTime.Now;
            Start = DateTime.Now;
            Finish = DateTime.Now;
        }

    }


    public class Results
    {
        [XmlElement(nameof(UnitTestResult))]
        public List<UnitTestResult> UnitTestResults { get; set; } = new List<UnitTestResult>();
    }
    public class TestDefinitions
    {
        [XmlElement(nameof(UnitTest))]
        public List<UnitTest> UnitTests { get; set; } = new List<UnitTest>();
    }

    public class UnitTest
    {
        public UnitTest()
        {
            
        }

        public UnitTest(string testname, string utrExecutionId,string testid)
        {
            Name = testname;
            Id = testid;
            Execution = new ExecutionInUnittest() {Id = utrExecutionId};
            TestMethod = new TestMethod(testname);
        }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("Execution")]
        public ExecutionInUnittest Execution { get; set; }


        public TestMethod TestMethod { get; set; }

        [XmlAttribute("storage")]
        public string Storage { get; set; }

        [XmlAttribute("id")]
        public string Id { get; set; }

    }

    public class TestMethod
    {
        [XmlAttribute("codeBase")]
        public string CodeBase { get; set; }
      
        [XmlAttribute("adapterTypeName")]
        public string AdapterTypeName { get; set; }
        [XmlAttribute("className")]
        public string ClassName { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }

        public TestMethod()
        {
            
        }

        public TestMethod(string name)
        {
            Name = name;
        }

    }
    public class TestEntries
    {
        [XmlElement(nameof(TestEntry))]
        public List<TestEntry> TestEntryList { get; set; } = new List<TestEntry>();
    }

    public class TestEntry
    {
        public TestEntry(string utrTestId, string utrExecutionId, string testListId)
        {
            TestId = utrTestId;
            ExecutionId = utrExecutionId;
            TestListId = testListId;
        }

        public TestEntry()
        {
            
        }

        [XmlAttribute("testId")]
        public string TestId { get; set; }
        [XmlAttribute("executionId")]
        public string ExecutionId { get; set; }
        [XmlAttribute("testListId")]
        public string TestListId { get; set; }

    }
    public class TestLists
    {
        [XmlElement(nameof(TestList))]
        public List<TestList> Lists { get; set; } = new List<TestList>();


        public TestLists()
        {
            Lists.Add(new TestList(false));
            Lists.Add(new TestList(true));
        }

        [XmlIgnore]
        public Guid GuidOfResultsNotInList => Lists.First(o => !o.AllLoadedResults).Guid;


    }

    public class TestList
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("id")]
        public string Id { get; set; }


        [XmlIgnore]
        public Guid Guid { get; set; } = Guid.NewGuid();

        public TestList()
        {
            Id = Guid.ToString();
        }

        [XmlIgnore]
        public bool AllLoadedResults { get; set; }

        public TestList(bool allLoadedResults) : this()
        {
            AllLoadedResults = allLoadedResults;
            Name = allLoadedResults ? "All Loaded Results" : "Results Not in a List";
        }

    }

    public class ResultSummary
    {
        [XmlAttribute("outcome")]
        public string OutCome { get; set; } = "Completed";

        public Counters Counters { get; set; } = new Counters();
        public Output Output { get; set; } = new Output();

        public void IncCounters(bool passed)
        {
            if (passed)
                Counters.IncPass();
            else
            {
                Counters.IncFail();
            }
        }


    }

    public class Counters
    {

        [XmlAttribute("total")]
        public int Total { get; set; }
        [XmlAttribute("executed")]
        public int Executed { get; set; }
        [XmlAttribute("passed")]
        public int Passed { get; set; }
        [XmlAttribute("failed")]
        public int Failed { get; set; }
        [XmlAttribute("error")]
        public int Error { get; set; }
        [XmlAttribute("timeout")]
        public int Timeout { get; set; }
        [XmlAttribute("aborted")]
        public int Aborted { get; set; }
        [XmlAttribute("inconclusive")]
        public int Inconclusive { get; set; }
        [XmlAttribute("passedButRunAborted")]
        public int PassedButRunAborted { get; set; }
        [XmlAttribute("notRunnable")]
        public int NotRunnable { get; set; }
        [XmlAttribute("notExecuted")]
        public int NotExecuted { get; set; }
        [XmlAttribute("disconnected")]
        public int Disconnected { get; set; }
        [XmlAttribute("warning")]
        public int Warning { get; set; }
        [XmlAttribute("completed")]
        public int Completed { get; set; }
        [XmlAttribute("inProgress")]
        public int InProgress { get; set; }
        [XmlAttribute("pending")]
        public int Pending { get; set; }


        public void IncPass()
        {
            Passed++;
            Total++;
            Executed++;
        }

        public void IncFail()
        {
            Failed++;
            Total++;
            Executed++;
        }


    }


    public class Execution
    {
        public TestTypeSpecific TestTypeSpecific { get; set; } = new TestTypeSpecific();
    }


    public class ExecutionInUnittest
    {
        [XmlAttribute("id")]
        public string Id { get; set; }
    }


    public class TestTypeSpecific
    {

    }

    public class Deployment
    {
        [XmlAttribute("runDeploymentRoot")]
        public string RunDeploymentRoot { get; set; }

        public Deployment()
        {

        }

        public Deployment(string filename)
        {
            RunDeploymentRoot = filename;
        }

    }

    public class Properties
    {

    }


    public class UnitTestResult
    {
        [XmlAttribute("executionId")]
        public string ExecutionId { get; set; }
        [XmlAttribute("testId")]
        public string TestId { get; set; }
        [XmlAttribute("testName")]
        public string TestName { get; set; }
        [XmlAttribute("computerName")]
        public string ComputerName { get; set; }
        [XmlAttribute("duration")]
        public string Duration { get; set; }
        [XmlAttribute("startTime")]
        public string StartTime { get; set; }
        [XmlAttribute("endTime")]
        public string EndTime { get; set; }
        [XmlAttribute("testType")]
        public string TestType { get; set; } = "13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b";
        [XmlAttribute("outcome")]
        public string Outcome { get; set; }
        [XmlAttribute("testListId")]
        public string TestListId { get; set; }
        [XmlAttribute("relativeResultsDirectory")]
        public string RelativeResultsDirectory { get; set; }


        public Output Output { get; set; }


        public UnitTestResult(string testname, bool passed, TimeInfo timeInfo, string output, string error,string testListId)
        {
            TestName = testname;
            var execuid = Guid.NewGuid().ToString();
            ExecutionId = execuid;
            var testid = Guid.NewGuid().ToString();
            TestId = testid;
            RelativeResultsDirectory = execuid;
            StartTime = timeInfo.Start.ToLongDateString();
            EndTime = timeInfo.End.ToLongDateString();
            Duration = timeInfo.Duration.ToString();
            Outcome = passed ? "Passed" : "Failed";
            bool anything = !string.IsNullOrEmpty(output) || !string.IsNullOrEmpty(error);
            if (anything)
            {
                Output = new Output(output, error);
            }
            TestListId = testListId;

        }


        public UnitTestResult()
        {

        }


    }

    public class Output
    {
        public Output(string output, string err)
        {
            StdOut = output;
            StdErr = err;
        }

        public Output()
        {

        }
        public string StdOut { get; set; }

        public string StdErr { get; set; }
    }






}
