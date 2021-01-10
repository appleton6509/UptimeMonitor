using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UptimeAPI.Controllers.DTOs
{
    public class PaginationDTO
    {
        public int MaxPageSize { get; set; } 
        public int RequestedPage { get; set; } 
        public int TotalPages { get; set; }
    }
}
