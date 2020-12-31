using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace UptimeAPI.Controllers
{
    public class APIBaseController : ControllerBase
    {
        public Guid UserId()
        {
            Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid userId);
            return userId;
        }
    }
}
