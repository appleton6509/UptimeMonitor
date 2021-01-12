
using System.ComponentModel.DataAnnotations;


namespace UptimeAPI.Controllers.DTOs
{
    public class WebEndPointDTO
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Ip { get; set; }
    }
}
