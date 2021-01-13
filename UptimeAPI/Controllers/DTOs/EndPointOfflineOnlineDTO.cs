using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UptimeAPI.Controllers.DTOs
{
    public class EndPointOfflineOnlineDTO
    {
        public string Ip { get; set; }
        public bool IsReachable { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
