using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UptimeAPI.Controllers.DTOs;
using UptimeAPI.Controllers.Helper;
using UptimeAPI.Controllers.QueryParams;
using UptimeAPI.Controllers.Repositories;
using UptimeAPI.Services;

namespace UptimeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultController : ControllerBase
    {
        #region Properties / Constructor
        private readonly IResultDataRepository _httpResultRepository;
        private readonly IEndPointRepository _endPointRepository;
        private readonly IAuthorizationService _authorizationService;


        public ResultController(
            IResultDataRepository httpResultRepository
            , IEndPointRepository endPointRepository
            , IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
            _httpResultRepository = httpResultRepository;
            _endPointRepository = endPointRepository;
        }
        #endregion

        #region Custom GET
        [HttpGet("Logs")]
        public ActionResult<List<ResultDataDetailsDTO>> GetAllResults([FromQuery] PaginationParam page, [FromQuery] ResultFilterParam filter)
        {
            if (page.RequestedPage > 0 & page.MaxPageSize > 0)
            {
                PagedList<ResultDataDetailsDTO> pagedList = (PagedList<ResultDataDetailsDTO>)_httpResultRepository.GetAll(page, filter);
                pagedList.AddPaginationToResponse(Response);
                return pagedList;
            }
            return _httpResultRepository.GetAll(page, filter);

        }
        [HttpGet("LogsByTime/{id}")]
        public async Task<ActionResult<List<ResultDataLatencyDTO>>> GetResultByEndPointByTime(Guid id, [FromQuery] TimeRangeParam range)
        {
            var endPoint = _endPointRepository.Get(id);
            AuthorizationResult auth = await _authorizationService.AuthorizeAsync(User, endPoint, Operations.Update);
            if (!auth.Succeeded)
                return new ForbidResult();

            return await _httpResultRepository.GetByEndPointAsync(id, range);
        }
        #endregion
    }
}
