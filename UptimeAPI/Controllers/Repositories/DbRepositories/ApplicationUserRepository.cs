using Data;
using Data.Models;
using Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using UptimeAPI.Controllers.DTOs;
using UptimeAPI.Services;
using UptimeAPI.Services.Email;
using UptimeAPI.Services.Token;

namespace UptimeAPI.Controllers.Repositories
{
    public class ApplicationUserRepository : BaseRepository, IApplicationUserRepository
    {
        private readonly JwtSettings _settings;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ApplicationUserRepository(
            UptimeContext context, 
            IHttpContextAccessor httpcontext,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IOptionsSnapshot<JwtSettings> jwtSettings) : base(context,httpcontext)
        {
            _userManager = userManager;
            _settings = jwtSettings.Value;
            _signInManager = signInManager;

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

        public async Task<ApplicationUser> Find(string email)
        {
            return await _userManager.FindByNameAsync(email);
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

        public async Task<ApplicationUser> Get(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<int> PutAsync(Guid id, UserDto model)
        {
            ApplicationUser user = await GetUser(id);

            if (user is null)
                return 0;

            IdentityResult changeEmail = await _userManager.SetEmailAsync(user, model.Username);
            IdentityResult changeUserName =await  _userManager.SetUserNameAsync(user, model.Username);

            if (changeEmail.Succeeded && changeUserName.Succeeded)
                return 1;
            return 0;
        }

        /// <summary>
        /// returns an url encoded token string
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> GenerateConfirmationToken(Guid id)
        {
            ApplicationUser user = await GetUser(id);
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);

        }

        public async Task<IdentityResult> ConfirmEmail(Guid id, string token)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id.ToString());
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        /// <summary>
        /// generates a password reset token
        /// </summary>
        /// <param name="id"></param>
        /// <returns>password reset token</returns>
        public async Task<string> GeneratePasswordResetToken(Guid id)
        {
            ApplicationUser user = await GetUser(id);
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }
        public async Task<IdentityResult> ResetPassword(UserResetDTO reset)
        {
            ApplicationUser user = await GetUser(reset.Id);
            return await _userManager.ResetPasswordAsync(user, reset.Token, reset.Password);
        }
        public async Task<IdentityResult> ChangePassword(UserChangePasswordDto details)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(details.Username);
           return await _userManager.ChangePasswordAsync(user, details.Password, details.NewPassword);
        }
        private async Task<ApplicationUser> GetUser(Guid id)
        {
            return await _userManager.FindByIdAsync(id.ToString());
        }
        

    }
}
