
using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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
        internal bool IsMatching(string id1, string id2)
        {
            if (String.Compare(id1.ToLower().Trim(), id2.ToLower().Trim()) != 0)
                return true;
            return false;
        }


    }
}
