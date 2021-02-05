using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UptimeAPI.Controllers.DTOs
{
    public class UserResetDTO
    {
        [Required]
        public Guid id { get; set; }
        [Required]
        public string password { get; set; }
        [Required]
        public string token { get; set; }
    }
}
