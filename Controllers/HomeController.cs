using Microsoft.AspNetCore.Mvc;
using TimeAnomalyWeb.Models;
using TimeAnomalyWeb.Services;

namespace TimeAnomalyWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var data = GenerateData();

            AnomalyDetector detector = new AnomalyDetector();
            var result = detector.Detect(data);

            return View(result);
        }

        private List<TimeData> GenerateData()
        {
            Random rand = new Random();
            var list = new List<TimeData>();

            for (int i = 0; i < 30; i++)
            {
                list.Add(new TimeData
                {
                    Timestamp = DateTime.Now.AddMinutes(i),
                    Value = rand.Next(10, 50)
                });
            }

            list[5].Value = 120;
            list[20].Value = 150;

            return list;
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}