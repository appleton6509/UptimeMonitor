using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UptimeAPI.Controllers.DTOs
{
    public class EndPointOfflineDTO
    {
        public string Ip { get; set; }
        public Guid Id { get; set; }
        public bool IsReachable { get; set; }
        public string Description { get; set; }
    }
}
