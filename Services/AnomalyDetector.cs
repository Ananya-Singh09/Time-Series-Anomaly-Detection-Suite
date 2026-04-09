using TimeAnomalyWeb.Models;

namespace TimeAnomalyWeb.Services
{
    public class AnomalyDetector
    {
        public List<TimeData> Detect(List<TimeData> data)
        {
            if (data == null || !data.Any())
                return data;

            double mean = data.Any() ? data.Average(x => x.Value) : 0;
            double std = data.Any() ? Math.Sqrt(data.Sum(x => Math.Pow(x.Value - mean, 2)) / data.Count) : 0;

            foreach (var d in data)
            {
                if (std == 0)
                {
                    d.IsAnomaly = false;
                    continue;
                }

                double z = (d.Value - mean) / std;
                d.IsAnomaly = Math.Abs(z) > 2.5;
            }

            return data;
        }
    }
}