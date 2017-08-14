using System;

namespace CheckTest
{
    public class TrxCounter
    {
        private readonly string content;
        public TrxCounter(string content)
        {
            this.content = content;
            Total = ExtractNumber("total");
            Executed = ExtractNumber("executed");
            Passed = ExtractNumber("passed");
            Failed = ExtractNumber("failed");
        }

        private  string ExtractNumberStr(string term)
        {
            int indexOfTerm = content.IndexOf(term+"=", StringComparison.InvariantCultureIgnoreCase);
            int lengthOfTerm = term.Length + 2;
            var startStr = content.Substring(indexOfTerm + lengthOfTerm);
            int indexOfEndOfTerm = startStr.IndexOf("\"", StringComparison.Ordinal);
            var termValueStr = content.Substring(indexOfTerm + lengthOfTerm, indexOfEndOfTerm);
            return termValueStr;
        }

        private int ExtractNumber(string term)
        {
            var str = ExtractNumberStr(term);
            return Convert.ToInt32(str);
        }

        public int Total { get; }
        public int Executed { get; }
        public int Passed { get; private set; }

        public int Failed { get; set; }
    }
}