using AutoMapper;
using Data;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UptimeAPI.Controllers.DTOs;
using UptimeAPI.Controllers.Helper;
using UptimeAPI.Controllers.QueryParams;
using UptimeAPI.Controllers.Repositories;
using UptimeAPI.Messaging;
using UptimeAPI.Services;

namespace UptimeAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ResultController : ApiBaseController
    {
        #region Properties / Constructor
        private readonly IAuthorizationService _authorizationService;
        private readonly IHttpResultRepository _httpResultRepository;
        private readonly IEndPointRepository _endPointRepository;


        public ResultController(
            IHttpResultRepository httpResultRepository
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
        public  ActionResult<List<EndPointDetailsDTO>> GetAllResults([FromQuery] PaginationParam page, [FromQuery] ResultFilterParam filter)
        {
            _httpResultRepository.GetAll(page, filter);

            if (page.RequestedPage > 0 & page.MaxPageSize > 0)
            {
                PagedList<EndPointDetailsDTO> pagedList = (PagedList<EndPointDetailsDTO>)_httpResultRepository.GetAll(page, filter);
                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(pagedList.Pagination));
                return pagedList;
            }
            return _httpResultRepository.GetAll(page, filter);

        }
        [HttpGet("LogsByTime/{id}")]
        public async Task<ActionResult<List<HttpResultLatencyDTO>>> GetResultByEndPointByTime(Guid id, [FromQuery] TimeRangeParam range)
        {
            EndPoint ep = _endPointRepository.Get(id);
            AuthorizationResult auth = await _authorizationService.AuthorizeAsync(User, ep, Operations.Update);
            if (!auth.Succeeded)
                return BadRequest(Error.Auth[AuthErrors.NoResourceAccess]);

            return await _httpResultRepository.GetByEndPoint(id, range);
        }
        #endregion
    }
}
