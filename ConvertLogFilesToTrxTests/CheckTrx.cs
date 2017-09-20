using System;
using System.Linq;
using ConvertLogFilesToTests;
using ConvertLogFilesToTrx;
using NUnit.Framework;

namespace ConvertLogFilesToTrxTests
{
    public class SerializationTest
    {
        const string ns ="microsoft.com/schemas/VisualStudio/TeamTest/2010";


        [Test]
        public void CheckSerializationOfDefaultTestRun()
        {
            var sut = new TestRun();
            var xml = sut.SerializeObjectWithOutNamespace(ns);
            Assert.That(xml.Contains("<Results"), "xml.Contains('<Results')");
            Assert.That(xml.Contains("<TestDefinitions"), "xml.Contains('<TestDefinitions')");
            Assert.That(xml.Contains("<TestEntries"), "xml.Contains('<TestEntries')");
            Assert.That(xml.Contains("<TestLists"), "xml.Contains('<TestLists')");
            Assert.That(xml.Contains("<ResultSummary"), "xml.Contains('<ResultSummary')");
            var resultsummary = xml.Substring(xml.IndexOf("<ResultSummary", StringComparison.InvariantCulture));
            Assert.That(resultsummary.Contains("<Counters"), "resultsummary.Contains('<Counters')");
            Assert.That(resultsummary.Contains("<Counters total"), "resultsummary.Contains('<Counters total')");
            Assert.That(resultsummary.Contains("<Output"), "resultsummary.Contains('<Output')");
            var testsettings = ExtractSegment(xml, "<TestSettings", "</TestSettings>");
            Assert.That(testsettings.Contains("<TestSettings name="), "testsettings.Contains('<TestSettings name=')");
            Assert.That(testsettings.Contains("<Execution>"), "testsettings.Contains('<Execution>')");
            Assert.That(testsettings.Contains("<Deployment"), "testsettings.Contains('<Deployment')");

            var testlists = ExtractSegment(xml, "<TestLists", "</TestLists>");
            Assert.That(testlists.Contains("<TestList name="), "testlists.Contains('<TestList name=')");
            Assert.That(testlists.Contains("Results Not in a List"), "testlists.Contains('Results Not in a List')");
            Assert.That(testlists.Contains("All Loaded Results"), "testlists.Contains('All Loaded Results')");


        }

        private string ExtractSegment(string xml, string start, string end)
        {
            var s = xml.IndexOf(start, StringComparison.InvariantCulture);
            Assert.That(s, Is.GreaterThanOrEqualTo(0), $"Can't find {start} in {xml}");
            var x = xml.Substring(s);
            var l = x.IndexOf(end, StringComparison.InvariantCulture);
            Assert.That(l, Is.GreaterThan(0), $"Can't find {end} after {start} in {xml}");
            x = x.Substring(0, l);
            return x;

        }



        [Test]
        public void CheckResultSummaryCounters()
        {
            var sut = new Counters();
            sut.IncPass();
            Assert.That(sut.Total, Is.EqualTo(1));
            Assert.That(sut.Passed, Is.EqualTo(1));
            Assert.That(sut.Executed, Is.EqualTo(1));
            sut.IncFail();
            Assert.That(sut.Passed, Is.EqualTo(1));
            Assert.That(sut.Failed, Is.EqualTo(1));
            Assert.That(sut.Total, Is.EqualTo(2));
            Assert.That(sut.Executed, Is.EqualTo(2));
        }


        [Test]
        public void CheckAddingResults()
        {
            var sut = new TestRun("someFileName", "myComputer");
            var time = new TimeInfo(DateTime.Now - new TimeSpan(500), DateTime.Now);
            sut.AddTestResult("MyTest", true, time, "All fine", "");

            Assert.That(sut.Results.UnitTestResults.Count, Is.EqualTo(1), "No results added to unittestresults");
            Assert.That(sut.TestDefinitions.UnitTests.Count, Is.EqualTo(1), "No testdefinitions added");
            Assert.That(sut.TestEntries.TestEntryList.Count, Is.EqualTo(1), "No test entries added");

            var utr = sut.Results.UnitTestResults.First();
            Assert.That(utr, Is.Not.Null, "UnitTestResult is empty/null");

            var xml = sut.SerializeObjectWithOutNamespace(ns);
            Assert.That(xml.Contains("UnitTestResults"), Is.False,
                "Contains UnitTestResults string, should only be array elements");
            Assert.That(xml.Contains("<UnitTestResult"), "xml.Contains('<UnitTestResult')");
            var tr = ExtractSegment(xml, "<UnitTestResult", "</UnitTestResult");
            Assert.That(tr.Contains("testName=\"MyTest"), "tr.Contains('testName=\'MyTest')");

        }

        [Test]
        public void CheckBothPassedAndFailedResults()
        {
            var sut = new TestRun("someFileName", "myComputer");
            var time = new TimeInfo(DateTime.Now - new TimeSpan(500), DateTime.Now);
            sut.AddTestResult("MyTest", true, time, "All fine", "");
            sut.AddTestResult("MySecondTest", false, time, "Didnt work", "Wrong");
            Assert.That(sut.Results.UnitTestResults.Count, Is.EqualTo(2), "No results added to unittestresults");
            Assert.That(sut.TestDefinitions.UnitTests.Count, Is.EqualTo(2), "No testdefinitions added");
            Assert.That(sut.TestEntries.TestEntryList.Count, Is.EqualTo(2), "No test entries added");

            var utr = sut.Results.UnitTestResults.First();
            Assert.That(utr, Is.Not.Null, "UnitTestResult is empty/null");

            Assert.That(sut.ResultSummary.Counters.Failed, Is.EqualTo(1));
            Assert.That(sut.ResultSummary.Counters.Passed, Is.EqualTo(1));
            Assert.That(sut.ResultSummary.Counters.Total, Is.EqualTo(2));

            var xml = sut.SerializeObjectWithOutNamespace(ns);
            Assert.That(xml.Contains("UnitTestResults"), Is.False,
                "Contains UnitTestResults string, should only be array elements");
            Assert.That(xml.Contains("<UnitTestResult"), "xml.Contains('<UnitTestResult')");
           

        }

        [Test]
        public void CheckAddingFailedResults()

      
        {
            var sut = new TestRun("someFileName", "myComputer");
            var time = new TimeInfo(DateTime.Now - new TimeSpan(500), DateTime.Now);
            sut.AddTestResult("MySecondTest", false, time, "Didnt work", "Wrong");
            Assert.That(sut.Results.UnitTestResults.Count, Is.EqualTo(1), "No results added to unittestresults");
            Assert.That(sut.TestDefinitions.UnitTests.Count, Is.EqualTo(1), "No testdefinitions added");
            Assert.That(sut.TestEntries.TestEntryList.Count, Is.EqualTo(1), "No test entries added");

            var utr = sut.Results.UnitTestResults.First();
            Assert.That(utr, Is.Not.Null, "UnitTestResult is empty/null");

            Assert.That(sut.ResultSummary.Counters.Failed, Is.EqualTo(1));

            var xml = sut.SerializeObjectWithOutNamespace(ns);
            Assert.That(xml.Contains("UnitTestResults"), Is.False,
                "Contains UnitTestResults string, should only be array elements");
            Assert.That(xml.Contains("<UnitTestResult"), "xml.Contains('<UnitTestResult')");
            var tr = ExtractSegment(xml, "<UnitTestResult", "</UnitTestResult");
            Assert.That(tr.Contains("testName=\"MySecondTest"), "tr.Contains('testName=\'MySecondTest')");

        }


        [Test]
        public void CheckNamespaces()
        {
            var sut = new TestRun("someFileName", "myComputer");

            var xml = sut.SerializeObjectWithOutNamespace(ns);
            Assert.Multiple(() =>
            {
                Assert.That(xml.Contains("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\""), Is.False);
                Assert.That(xml.Contains("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\""), Is.False);
                Assert.That(xml.Contains("xmlns=\"http://microsoft.com/schemas/VisualStudio/TeamTest/2010\""), Is.True);
            });

        }
    }
}