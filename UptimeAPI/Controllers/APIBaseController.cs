
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace UptimeAPI.Controllers
{
    public class ApiBaseController : ControllerBase
    {

        [ApiExplorerSettings(IgnoreApi = true)]
        public Guid UserId()
        {
            Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid userId);
            return userId;
        }
    }
}
