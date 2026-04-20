using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using TimeAnomalyWeb.Models;
using TimeAnomalyWeb.Services;

namespace TimeAnomalyWeb.Controllers
{
    public class HomeController : Controller
    {
        private static List<TimeData> _dataStore = new List<TimeData>();
        private readonly AnomalyDetector _detector = new AnomalyDetector();

        public IActionResult Index()
        {
            return View(_dataStore);
        }

        [HttpPost]
        public IActionResult UploadCSV(IFormFile file)
        {
            var rawData = new List<TimeData>();
            var values = new List<double>();

            if (file == null || file.Length == 0)
            {
                Console.WriteLine("NO FILE RECEIVED");
                return RedirectToAction("Index");
            }

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                int lineNumber = 0;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    lineNumber++;

                    Console.WriteLine($"LINE {lineNumber}: {line}");

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    if (lineNumber == 1)
                        continue; // skip header

                    var parts = line.Split(',');

                    if (parts.Length < 2)
                    {
                        Console.WriteLine("INVALID FORMAT");
                        continue;
                    }

                    string timeStr = parts[0].Trim();
                    string valStr = parts[1].Trim();

                    Console.WriteLine($"TIME: {timeStr}, VALUE: {valStr}");

                    if (!DateTime.TryParse(timeStr, out DateTime timestamp))
                    {
                        Console.WriteLine("DATE PARSE FAILED");
                        continue;
                    }

                    if (!double.TryParse(valStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                    {
                        Console.WriteLine("VALUE PARSE FAILED");
                        continue;
                    }

                    rawData.Add(new TimeData
                    {
                        Timestamp = timestamp,
                        Value = value
                    });

                    values.Add(value);
                }
            }

            Console.WriteLine($"TOTAL PARSED: {rawData.Count}");

            if (values.Count == 0)
            {
                Console.WriteLine("NO VALID DATA");
                return RedirectToAction("Index");
            }

            var detected = _detector.Detect(values);

            for (int i = 0; i < rawData.Count; i++)
            {
                rawData[i].IsAnomaly = detected[i].IsAnomaly;
            }

            // 🔥 IMPORTANT: RETURN VIEW DIRECTLY (NO REDIRECT)
            return View("Index", rawData);
        }
    }
}