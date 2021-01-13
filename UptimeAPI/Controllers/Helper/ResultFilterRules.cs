
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UptimeAPI.Controllers.DTOs;
using UptimeAPI.Controllers.QueryParams;

namespace UptimeAPI.Controllers.Helper
{
    public class ResultFilterRules
    {
        private ResultFilterParam _filter;
        public IQueryable<EndPointDetailsDTO> Query 
        {
            get
            {
                return this._query;
            }  
        }
        private IQueryable<EndPointDetailsDTO> _query;
        public ResultFilterRules(ResultFilterParam filter, IQueryable<EndPointDetailsDTO> query)
        {
            _filter = filter;
            _query = query;
        }
        public IQueryable<EndPointDetailsDTO> ApplyFilters()
        {
            return this.FilterReachable().FilterSortBy()._query;
        }
        private ResultFilterRules FilterReachable()
        {
            if (_filter.Reachable.HasValue)
                _query = _query.Where(x => x.IsReachable == _filter.Reachable.Value);
            return this;
        }
        private ResultFilterRules FilterSortBy()
        {
           return this.SortByDescending().SortByAscending();
        }

        private ResultFilterRules SortByDescending()
        {
            if (_filter.OrderBy == OrderBy.Descending || _filter.OrderBy == OrderBy.None)
            {
                switch (_filter.SortBy)
                {
                    case SortBy.Description:
                        _query = _query.OrderByDescending(x => x.Description);
                        break;
                    case SortBy.Latency:
                        _query = _query.OrderByDescending(x => x.Latency);
                        break;
                    case SortBy.Reachable:
                        _query = _query.OrderByDescending(x => x.IsReachable);
                        break;
                    case SortBy.Site:
                        _query = _query.OrderByDescending(x => x.Ip);
                        break;
                    case SortBy.Timestamp:
                        _query = _query.OrderByDescending(x => x.TimeStamp);
                        break;
                    default:
                        _query = _query.OrderByDescending(x => x.TimeStamp);
                        break;
                }
            }
            return this;
        }
        private ResultFilterRules SortByAscending()
        {
            if (_filter.OrderBy == OrderBy.Ascending)
            {
                switch (_filter.SortBy)
                {
                    case SortBy.Description:
                        _query = _query.OrderBy(x => x.Description);
                        break;
                    case SortBy.Latency:
                        _query = _query.OrderBy(x => x.Latency);
                        break;
                    case SortBy.Reachable:
                        _query = _query.OrderBy(x => x.IsReachable);
                        break;
                    case SortBy.Site:
                        _query = _query.OrderBy(x => x.Ip);
                        break;
                    case SortBy.Timestamp:
                        _query = _query.OrderBy(x => x.TimeStamp);
                        break;
                    default:
                        _query = _query.OrderBy(x => x.TimeStamp);
                        break;
                }
            }
            return this;
        }
    }
}
