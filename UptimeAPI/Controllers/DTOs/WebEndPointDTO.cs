
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace UptimeAPI.Controllers.DTOs
{
    public class WebEndPointDTO
    {
        public string Id { get; set; }

        public string Description { get; set; }

        [Display(Name = "Site / IP")]
        [StringLength(5)]
        public string Ip { get; set; }

    }
}
