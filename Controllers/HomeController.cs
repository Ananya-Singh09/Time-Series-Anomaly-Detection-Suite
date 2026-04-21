using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json;

namespace SimpleAnomalyDetector.Controllers;

public class HomeController : Controller
{
    private static List<TimeData> _data = new();

    public IActionResult Index()
    {
        return View(_data);
    }

    [HttpPost]
    public IActionResult Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "No file selected";
            return RedirectToAction("Index");
        }

        var values = new List<double>();

        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            string line;
            int lineNum = 0;
            while ((line = reader.ReadLine()) != null)
            {
                lineNum++;
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (lineNum == 1 && line.ToLower().Contains("timestamp")) continue;

                var parts = line.Split(',');
                var valueStr = parts.Length == 1 ? parts[0] : parts[1];

                if (double.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
                {
                    values.Add(val);
                }
            }
        }

        if (values.Count == 0)
        {
            TempData["Error"] = "No valid numbers found";
            return RedirectToAction("Index");
        }

        var mean = values.Average();
        var stdDev = Math.Sqrt(values.Select(v => Math.Pow(v - mean, 2)).Average());
        if (stdDev == 0) stdDev = 1;

        _data = values.Select((v, i) => new TimeData
        {
            Id = i + 1,
            Timestamp = DateTime.Now.AddSeconds(i),
            Value = v,
            IsAnomaly = Math.Abs((v - mean) / stdDev) > 2.0
        }).ToList();

        TempData["Success"] = $"Loaded {_data.Count} records, {_data.Count(x => x.IsAnomaly)} anomalies";
        return RedirectToAction("Index");
    }

    public IActionResult Clear()
    {
        _data.Clear();
        TempData["Success"] = "Data cleared";
        return RedirectToAction("Index");
    }
}

public class TimeData
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public double Value { get; set; }
    public bool IsAnomaly { get; set; }
}