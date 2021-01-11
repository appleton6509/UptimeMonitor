
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using Data.Models;

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

        [ApiExplorerSettings(IgnoreApi = true)]
        internal bool OwnsModel(Guid modelUserId)
        { 
            Guid userId = UserId();
            if (!Object.Equals(userId,null) && !Object.Equals(modelUserId, null) && userId.Equals(modelUserId))
                return true;
            return false;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        internal bool IsMatching(string id1, string id2)
        {
            if (id1.Trim().Equals(id2.Trim(),StringComparison.OrdinalIgnoreCase))
                return true;
            return false;
        }


    }
}
