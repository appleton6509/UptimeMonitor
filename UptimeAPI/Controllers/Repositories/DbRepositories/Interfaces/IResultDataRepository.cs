using Data.Models;
using Data.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UptimeAPI.Controllers.DTOs;
using UptimeAPI.Controllers.QueryParams;

namespace UptimeAPI.Controllers.Repositories
{
    public interface IResultDataRepository : IRepository<ResultData>
    {
        List<EndPointDetailsDTO> GetAll(PaginationParam page, ResultFilterParam filter);
        Task<List<ResultDataLatencyDTO>> GetByEndPointAsync(Guid id, TimeRangeParam range);
    }
}