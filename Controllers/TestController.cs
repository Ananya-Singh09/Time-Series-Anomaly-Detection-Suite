using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TimeAnomalyWeb.Models;

namespace TimeAnomalyWeb.Controllers
{
    public class TestController : Controller
    {
        private static List<TimeData> data = new List<TimeData>();

        public IActionResult Index()
        {
            return View(data);
        }

        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            if (file == null)
            {
                ViewBag.Error = "No file";
                return View("Index", data);
            }

            var list = new List<TimeData>();
            var values = new List<double>();

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                string line;
                int i = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    i++;
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    if (i == 1 && line.Contains("Timestamp")) continue; // skip header

                    string[] parts = line.Split(',');
                    double val;

                    if (parts.Length == 1)
                    {
                        if (double.TryParse(parts[0], NumberStyles.Any, CultureInfo.InvariantCulture, out val))
                        {
                            values.Add(val);
                            list.Add(new TimeData { Timestamp = DateTime.Now.AddSeconds(i), Value = val });
                        }
                    }
                    else if (parts.Length >= 2)
                    {
                        if (double.TryParse(parts[1], NumberStyles.Any, CultureInfo.InvariantCulture, out val))
                        {
                            values.Add(val);
                            list.Add(new TimeData { Timestamp = DateTime.Now.AddSeconds(i), Value = val });
                        }
                    }
                }
            }

            if (values.Count == 0)
            {
                ViewBag.Error = "No numbers found";
                return View("Index", data);
            }

            
            double mean = values.Average();
            double stdDev = Math.Sqrt(values.Select(v => Math.Pow(v - mean, 2)).Average());
            if (stdDev == 0) stdDev = 1;

            for (int i = 0; i < list.Count; i++)
            {
                double zScore = Math.Abs((list[i].Value - mean) / stdDev);
                list[i].IsAnomaly = zScore > 2.0;
            }

            data = list;
            ViewBag.Success = $"Loaded {list.Count} records, {list.Count(x => x.IsAnomaly)} anomalies";

            return RedirectToAction("Index");
        }

        public IActionResult Clear()
        {
            data.Clear();
            return RedirectToAction("Index");
        }
    }
}