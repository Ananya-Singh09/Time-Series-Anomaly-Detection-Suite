namespace TimeAnomalyWeb.Models;

public class TimeData
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public double Value { get; set; }
    public bool IsAnomaly { get; set; }
}