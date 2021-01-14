
using System.ComponentModel.DataAnnotations;


namespace UptimeAPI.Controllers.DTOs
{
    public class WebEndPointDTO
    {
        public string Id { get; set; }

        public string Description { get; set; }

        public string Ip { get; set; }
    }
}
