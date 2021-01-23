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
    public class WebUserRepository : BaseRepository, IWebUserRepository
    {
        public WebUserRepository(UptimeContext context, IHttpContextAccessor httpcontext)
            : base(context, httpcontext) { }
        
        public WebUser Get(string identityId)
        {
            return _context.WebUser.FirstOrDefault(x => x.IdentityId.Equals(identityId));
        }
        public Task<int> PostAsync(WebUser model)
        {
            _context.Add(model);
            return _context.SaveChangesAsync();
        }

        public List<WebUser> GetAll()
        {
            throw new NotImplementedException();
        }


        public Task<int> PutAsync(Guid id, WebUser model)
        {
            throw new NotImplementedException();
        }

        public WebUser Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool Exists(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
