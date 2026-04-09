using Microsoft.AspNetCore.Mvc;
using TimeAnomalyWeb.Models;
using TimeAnomalyWeb.Services;

namespace TimeAnomalyWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View(new List<TimeData>());
        }

        [HttpPost]
        public IActionResult Upload(IFormFile? file)
        {
            var data = new List<TimeData>();

            if (file != null && file.Length > 0)
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        if (string.IsNullOrWhiteSpace(line))
                            continue;

                        var values = line.Split(',');

                        if (values.Length < 2)
                            continue;

                        if (DateTime.TryParse(values[0], out DateTime time) &&
                            double.TryParse(values[1], out double val))
                        {
                            data.Add(new TimeData
                            {
                                Timestamp = time,
                                Value = val
                            });
                        }
                    }
                }
            }

            var detector = new AnomalyDetector();
            var result = detector.Detect(data) ?? new List<TimeData>();

            TempData["LastUpload"] = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

            return View("Index", result);
        }
    }
}