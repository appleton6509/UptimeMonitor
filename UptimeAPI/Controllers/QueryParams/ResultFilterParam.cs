using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UptimeAPI.Controllers.QueryParams
{
    public enum OrderBy
    {
        None,
        Descending,
        Ascending
    }
    public enum SortBy
    {
        None,
        Timestamp,
        Latency,
        Site,
        Description,
        Reachable
    }
    public class ResultFilterParam
    {
        public bool? Reachable { get; set; }
        public OrderBy OrderBy { get; set; } = OrderBy.None;
        public SortBy SortBy { get; set; } = SortBy.None;
    }
}
