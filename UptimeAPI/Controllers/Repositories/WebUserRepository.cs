using Data;
using Data.Models;
using Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UptimeAPI.Controllers.Repositories
{
    public class WebUserRepository : BaseRepository<WebUser>, IWebUserRepository
    {
        public WebUserRepository(UptimeContext context, IHttpContextAccessor httpcontext, IAuthorizationService auth) : base(context, httpcontext, auth)
        {
        }
        public WebUser Get(string identityId)
        {
            return _context.WebUser.FirstOrDefault(x => x.IdentityId.Equals(identityId));
        }
        public override Task<int> PostAsync(WebUser model)
        {
            _context.Add(model);
            return _context.SaveChangesAsync();
        }

        public override Task<List<WebUser>> GetAllAsync()
        {
            throw new NotImplementedException();
        }


        public override Task<int> PutAsync(Guid id, WebUser model)
        {
            throw new NotImplementedException();
        }
    }
}
