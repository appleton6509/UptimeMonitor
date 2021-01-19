using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T> where T : BaseModel
    {
        #region Properties/Constructor
        internal readonly UptimeContext _context;
        private DbSet<T> _model;
        internal IHttpContextAccessor _httpContext;
        internal readonly IAuthorizationService _authorizationService;
        internal readonly Guid _userId;
        public BaseRepository(UptimeContext context, IHttpContextAccessor httpcontext, IAuthorizationService auth)
        {
            _context = context;
            _httpContext = httpcontext;
            _authorizationService = auth;
            _model = _context.Set<T>();
            _userId = UserId();
        }
        #endregion

        #region CRUD
        public abstract Task<int> PutAsync(Guid id, T model);
        public abstract Task<int> PostAsync(T model);
        public abstract Task<List<T>> GetAllAsync();
        public virtual void Delete(Guid id)
        {
            var model = _model.Find(id);
            _model.Remove(model);
            _context.SaveChanges();
        }
        public virtual T Get(Guid id)
        {
            return _model.Find(id);
        }
        #endregion

        public virtual bool Exists(Guid id)
        {
            return _model.Any(k => k.Id == id);
        }
        public Guid UserId()
        {
            var IsSuccess = Guid.TryParse(_httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid userId);
            if (!IsSuccess)
                return Guid.Empty;
            return userId;
        }

    }
}
