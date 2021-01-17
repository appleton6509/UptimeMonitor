using System;
using System.ComponentModel.DataAnnotations;

namespace UptimeAPI.Controllers.DTOs
{
    public class WebUserDTO
    {
        public Guid Id { get; set; }
        [Required]
        [RegularExpression(@"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?",
            ErrorMessage = "{0} requires a valid email address")]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
