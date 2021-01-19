using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UptimeAPI.Controllers.DTOs;
using UptimeAPI.Controllers.QueryParams;

namespace UptimeAPI.Controllers.Repositories
{
    public interface IHttpResultRepository
    {
        List<EndPointDetailsDTO> GetAll(PaginationParam page, ResultFilterParam filter);
        Task<List<HttpResultLatencyDTO>> GetByEndPoint(Guid id, TimeRangeParam range);
    }
}