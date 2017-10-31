namespace CheckTestLog.Test
{
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
        public string StackTrace { get; set; } = "";
        public string Message { get; set; }

        public string Test { get; set; }
    }
}