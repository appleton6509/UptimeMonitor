﻿using System;
using System.Collections.Generic;
using System.Text;

namespace UptimeAPI.Controllers.DTOs
{
       
    public class EndPointDetailsDTO
    {
        public string Ip { get; set; }
        public bool IsReachable { get; set; }
        public string Description { get; set; }
        public Guid Id { get; set; }
        public int Latency { get; set; }
        public string Status { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
