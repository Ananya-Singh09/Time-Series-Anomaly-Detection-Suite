using System;
using System.Collections.Generic;
using System.Linq;
using TimeAnomalyWeb.Models;

namespace TimeAnomalyWeb.Services
{
    public class AnomalyDetector
    {
        private const double Z_THRESHOLD = 2.0;

        public List<TimeData> Detect(List<double> values)
        {
            var result = new List<TimeData>();

            if (values == null || values.Count == 0)
                return result;

            double mean = values.Average();
            double variance = values.Select(v => Math.Pow(v - mean, 2)).Average();
            double stdDev = Math.Sqrt(variance);

            if (stdDev == 0)
                stdDev = 1;

            for (int i = 0; i < values.Count; i++)
            {
                double zScore = (values[i] - mean) / stdDev;

                result.Add(new TimeData
                {
                    Timestamp = DateTime.Now.AddSeconds(i),
                    Value = values[i],
                    IsAnomaly = Math.Abs(zScore) > Z_THRESHOLD
                });
            }

            return result;
        }
    }
}