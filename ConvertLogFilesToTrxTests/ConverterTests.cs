using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConvertLogFilesToTrx;
using NSubstitute;
using NUnit.Framework;
using WrapThat.SystemIO;

namespace ConvertLogFilesToTrxTests
{
    public class ConverterTests
    {
        private const string txt1 = "whatever\r\n::BEGIN::Model1::ahahahah\r\n::Success::END::\r\nahahahah";
        private const string txt2 = "whatever\r\n::BEGIN::Model1::::Success::END::\r\nahahahah";
        private const string txt3 = "whatever\r\n::BEGIN::Model1::Some huge error message\r\n::Failed::END::\r\nahahahah";



        [TestCase(txt2, 1)]
        [TestCase(txt1, 1)]
        public void TestTextBlockSplit(string inp, int expected)
        {
            var sut = new Converter("whatever", false);
            var file = Substitute.For<IFile>();
            //  file.ReadAllText(Arg.Any<string>()).Returns(inp);
            var txt = sut.ExtractBlocks(inp);
            Assert.That(txt.Count, Is.EqualTo(expected));
        }
        [TestCase(txt3, 1, false)]
        [TestCase(txt2, 1, true)]
        [TestCase(txt1, 1, true)]
        public void TestTextBlockExtracts(string inp, int expected, bool outcome)
        {
            var sut = new Converter("whatever", false);
            var file = Substitute.For<IFile>();
            //  file.ReadAllText(Arg.Any<string>()).Returns(inp);
            var txt = sut.ExtractBlocks(inp);
            Assert.That(txt.Count, Is.EqualTo(expected));
            var tests = sut.ExtractTestStrings(txt);
            foreach (var test in tests)
            {
                Assert.That(test.Name == "Model1");
              
                if (!outcome)
                {
                    Assert.That(test.Outcome, Is.EqualTo("Failed"));
                    Assert.That(test.Msg,Is.EqualTo("Some huge error message\r\n"));
                }
                else
                {
                    Assert.That(test.Outcome, Is.EqualTo("Success"));
                }

            }
        }
    }
}
