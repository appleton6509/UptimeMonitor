using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data.Models;
using UptimeAPI.Messaging;
using UptimeAPI.Controllers.DTOs;
using Microsoft.AspNetCore.Authorization;
using UptimeAPI.Services;
using UptimeAPI.Controllers.Repositories;
using Microsoft.Extensions.Logging;

namespace UptimeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EndPointsController : ControllerBase
    {
        #region Properties  / Constructor
        private readonly IAuthorizationService _authorizationService;
        private readonly IEndPointRepository _endPointRepository;
        private readonly ILogger<EndPointsController> _logger;


        public EndPointsController(
            ILogger<EndPointsController> logger
            , IAuthorizationService authorizationService
            , IEndPointRepository endPoint)
        {
            _authorizationService = authorizationService;
            _endPointRepository = endPoint;
            _logger = logger;
        }
        #endregion

        #region Basic CRUD
        // GET: api/EndPoints
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EndPoint>>> GetAllEndPoints()
        {
            return await _endPointRepository.GetAllAsync();
        }

        // PUT: api/EndPoints/5DFBBC20-D61E-4506-58DE-08D8B0516C01
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEndPoint(Guid id, EndPoint endPoint)
        {

            EndPoint ep = _endPointRepository.Get(id);
            AuthorizationResult auth = await _authorizationService.AuthorizeAsync(User, ep, Operations.Update);
            if (!auth.Succeeded)
            {

                return BadRequest(Error.Auth[AuthErrors.NoResourceAccess]);
            }


            try
            {
                await _endPointRepository.PutAsync(id, endPoint);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_endPointRepository.Exists(id)) 
                    NotFound(); 
                else 
                    throw;
            }

            return NoContent();
        }

        // POST: api/EndPoints
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<EndPoint>> PostEndPoint(EndPoint endPoint)
        {
            try
            {
                await _endPointRepository.PostAsync(endPoint);
            } catch
            {
                return BadRequest();
            }

            return Ok();
        }

        // DELETE: api/EndPoints/5DFBBC20-D61E-4506-58DE-08D8B0516C01
        [HttpDelete("{id}")]
        public async Task<ActionResult<EndPoint>> DeleteEndPoint(Guid id)
        {
            var endPoint = _endPointRepository.Get(id);

            AuthorizationResult auth = await _authorizationService.AuthorizeAsync(User, endPoint, Operations.Delete);
            if (!auth.Succeeded)
                return BadRequest(Error.Auth[AuthErrors.NoResourceAccess]);

            _endPointRepository.Delete(id);

            return Ok();
        }
        #endregion Basic CRUD

        #region Custom

        // GET: api/EndPoints/Offline
        [HttpGet("Offline")]
        public async Task<ActionResult<List<EndPointOfflineDTO>>> GetOfflineEndPoints()
        {
            return await _endPointRepository.GetOfflineEndPointsAsync();
        }
        // GET: api/EndPoints/ConnectionStatus
        [HttpGet("ConnectionStatus")]
            public async Task<ActionResult<List<EndPointOfflineOnlineDTO>>> GetEndPointsStatus()
        {
            return await _endPointRepository.GetEndPointsStatus();
        }

        //api/EndPoints/Statistics
        [HttpGet("Statistics")]
        public ActionResult<List<EndPointStatisticsDTO>> GetAllEndPointStatistics()
        {
            return _endPointRepository.GetEndPointStatistics();
        }

        //api/EndPoints/Statistics/5DFBBC20-D61E-4506-58DE-08D8B0516C01
        [HttpGet("Statistics/{id}")]
        public async Task<ActionResult<EndPointStatisticsDTO>> GetEndPointStatistics(Guid id)
        {
            var endPoint = _endPointRepository.Get(id);
            AuthorizationResult auth = await _authorizationService.AuthorizeAsync(User, endPoint, Operations.Update);
            if (!auth.Succeeded)
                return BadRequest(Error.Auth[AuthErrors.NoResourceAccess]);

            return _endPointRepository.GetEndPointStatistics(endPoint);
        }
        #endregion Custom
    }
}
