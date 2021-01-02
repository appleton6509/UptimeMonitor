﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class HttpResult
    {
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public bool IsReachable { get; set; }
        public int Latency { get; set; }
        public string StatusMessage { get; set; }
        public Guid EndPointId { get; set; }
    }
}