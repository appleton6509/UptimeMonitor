using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace Data.Repositories
{
    public abstract class BaseRepository
    {
        internal IHttpContextAccessor _httpContext;
        internal UptimeContext _context;
        internal IMapper _mapper;
        public BaseRepository(UptimeContext context, IHttpContextAccessor httpcontext, IMapper mapper = null)
        {
            _httpContext = httpcontext;
            _context = context;
            _mapper = mapper;
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