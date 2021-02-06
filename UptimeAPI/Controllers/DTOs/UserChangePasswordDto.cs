using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UptimeAPI.Controllers.DTOs
{
    public class UserChangePasswordDto
    {
        [Required]
        [Display(Name = "Email")]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }
    }
}
