using System.ComponentModel.DataAnnotations;

namespace TimeAnomalyWeb.Models
{
    public class UploadHistory
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public DateTime UploadTime { get; set; }

        public int TotalRecords { get; set; }

        public int Anomalies { get; set; }
    }
}