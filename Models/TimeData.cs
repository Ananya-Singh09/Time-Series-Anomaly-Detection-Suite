namespace TimeAnomalyWeb.Models
{
    public class TimeData
    {
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
        public bool IsAnomaly { get; set; } // 🔥 THIS WAS MISSING
    }
}