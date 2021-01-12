
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;


namespace UptimeAPI.Controllers
{

    public class ApiBaseController : ControllerBase
    {
        [ApiExplorerSettings(IgnoreApi = true)]
        internal Guid UserId()
        {
            Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid userId);
            return userId;
        }
    }
}
