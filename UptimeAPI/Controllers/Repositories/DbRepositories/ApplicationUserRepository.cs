using Data;
using Data.Models;
using Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using UptimeAPI.Controllers.DTOs;
using UptimeAPI.Services;
using UptimeAPI.Settings;

namespace UptimeAPI.Controllers.Repositories
{
    public class ApplicationUserRepository : BaseRepository, IApplicationUserRepository
    {
        private readonly JwtSettings _settings;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationUserRepository(UptimeContext context, IHttpContextAccessor httpcontext, UserManager<ApplicationUser> userManager
            , IOptionsSnapshot<JwtSettings> jwtSettings) : base(context,httpcontext)
        {
            _userManager = userManager;
            _settings = jwtSettings.Value;
        }

        public async Task<bool> Exists(string userName)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(userName);
            if (user != null)
                return true;
            return false;
        }
        public async Task<IdentityResult> PostAsync(UserDto user)
        {
            ApplicationUser newUser = new ApplicationUser(user.Username);
            return await _userManager.CreateAsync(newUser, user.Password);
        }
        public async Task<bool> ValidatePassword(UserDto user)
        {
            var username = await _userManager.FindByNameAsync(user.Username);
            return await _userManager.CheckPasswordAsync(username, user.Password);
        }
        public async Task<string> SignIn(UserDto user)
        {
            ApplicationUser dbUser = await _userManager.FindByNameAsync(user.Username);
            if (dbUser != null)
                return new JwtTokenService().GenerateToken(dbUser, _settings);
            return String.Empty;
        }
        public ApplicationUser Get(Guid id)
        {
            return _context.ApplicationUser.FirstOrDefault(x => x.Id.Equals(id));
        }
        public async Task<int> PutAsync(Guid id, UserDto model)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id.ToString());

            if (user is null)
                return 0;

            IdentityResult changeEmail = await _userManager.SetEmailAsync(user, model.Username);
            IdentityResult changeUserName =await  _userManager.SetUserNameAsync(user, model.Username);

            if (changeEmail.Succeeded && changeUserName.Succeeded)
                return 1;
            return 0;
        }

    }
}
