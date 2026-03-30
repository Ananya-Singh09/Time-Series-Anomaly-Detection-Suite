using TimeAnomalyWeb.Models;

namespace TimeAnomalyWeb.Services
{
    public class AnomalyDetector
    {
        public List<TimeData> Detect(List<TimeData> data)
        {
            double mean = data.Average(x => x.Value);
            double std = Math.Sqrt(data.Sum(x => Math.Pow(x.Value - mean, 2)) / data.Count);

            foreach (var d in data)
            {
                double z = (d.Value - mean) / std;
                d.IsAnomaly = Math.Abs(z) > 2.5;
            }

            return data;
        }
    }
}