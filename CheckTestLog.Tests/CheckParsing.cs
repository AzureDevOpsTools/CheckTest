using System.IO;
using System.Linq;
using CheckTestLog.Test;
using NUnit.Framework;

namespace CheckTestLog.Tests
{
    public class CheckParsing
    {
        [Test]
        public void CheckParsingFile()
        {
            var dir = TestContext.CurrentContext.TestDirectory;
            var path = Path.Combine(dir, "dorytest.log");
            var p = new LogParser(File.ReadAllLines(path));

            var exc = p.Exceptions;
            Assert.That(exc.Count(),Is.EqualTo(1));
            var ex = exc.First();
            Assert.Multiple(() =>
            {
                Assert.That(ex.Type, Is.EqualTo("System.ArgumentNullException"));
                Assert.That(ex.Message, Is.EqualTo("Value cannot be null."));
                Assert.That(ex.StackTrace.Length, Is.GreaterThan(100));
                Assert.That(ex.Test,Is.EqualTo("ParallelTestExecutorServiceClient"));
            });


        }
    }
}
