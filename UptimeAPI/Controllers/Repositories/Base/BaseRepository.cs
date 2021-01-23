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