using System;
using System.Collections.Generic;
using System.Linq;
using TimeAnomalyWeb.Models;

namespace TimeAnomalyWeb.Services
{
    public class AnomalyDetector
    {
        public List<TimeData> Detect(List<double> values)
        {
            var result = new List<TimeData>();

            if (values == null || values.Count == 0)
                return result;

            // Calculate mean and standard deviation
            double mean = values.Average();
            double sumOfSquares = 0;

            for (int i = 0; i < values.Count; i++)
            {
                sumOfSquares += Math.Pow(values[i] - mean, 2);
            }

            double stdDev = Math.Sqrt(sumOfSquares / values.Count);

            if (stdDev == 0)
                stdDev = 1;

            // Detect anomalies using Z-Score method
            for (int i = 0; i < values.Count; i++)
            {
                double zScore = Math.Abs((values[i] - mean) / stdDev);
                bool isAnomaly = zScore > 2.0; // Values beyond 2 standard deviations are anomalies

                result.Add(new TimeData
                {
                    Timestamp = DateTime.Now.AddSeconds(i),
                    Value = values[i],
                    IsAnomaly = isAnomaly
                });
            }

            return result;
        }
    }
}