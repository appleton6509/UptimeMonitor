using System;


namespace UptimeAPI.Controllers.DTOs
{
    public class WebUserDTO
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
