using System;
using System.Collections.Generic;

namespace Data.Models
{
    public class EndPoint 
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Description { get; set; }
        public string Ip { get; set; }

        public virtual ICollection<HttpResult> HttpResult { get; set; }

    }
}