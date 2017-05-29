using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CheckTest.Test
{
    public class CheckTrxReading
    {
        [Test]
        public void ThatWeCanParseCounterString()
        {
            const string counterStr = "    <Counters total=\"75\" executed=\"74\" passed=\"73\" failed=\"1\" error=\"2\" timeout=\"3\" aborted=\"0\" inconclusive=\"0\" passedButRunAborted=\"0\" notRunnable=\"0\" notExecuted=\"0\" disconnected=\"0\" warning=\"0\" completed=\"0\" inProgress=\"0\" pending=\"0\" />";

            var sut = new TrxCounter(counterStr);
            Assert.That(sut.Total,Is.EqualTo(75));
            Assert.That(sut.Executed,Is.EqualTo(74));
            Assert.That(sut.Passed,Is.EqualTo(73));
            Assert.That(sut.Failed,Is.EqualTo(1));

        }
    }
}
