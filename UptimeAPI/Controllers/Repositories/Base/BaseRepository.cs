using AutoMapper;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
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
        private readonly DbSet<T> _model;
        internal IHttpContextAccessor _httpContext;
        internal readonly Guid _userId;
        public BaseRepository(UptimeContext context, IHttpContextAccessor httpcontext)
        {
            _context = context;
            _httpContext = httpcontext;
            _model = _context.Set<T>();
            _userId = UserId();
        }
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
