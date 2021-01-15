using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UptimeAPI.Controllers.QueryParams
{
    public class PaginationParam
    {

        public int MaxPageSize { get; set; }

        public int RequestedPage { get; set; }
    }
}
