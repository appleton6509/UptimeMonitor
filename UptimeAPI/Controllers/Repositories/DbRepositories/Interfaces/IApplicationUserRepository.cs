using Data.Models;
using Data.Repositories;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;
using UptimeAPI.Controllers.DTOs;

namespace UptimeAPI.Controllers.Repositories
{
    public interface IApplicationUserRepository 
    {
        Task<int> PutAsync(Guid id, UserDto model);
        Task<IdentityResult> PostAsync(UserDto user);
        Task<bool> Exists(string userName);
        /// <summary>
        /// signs a user in and returns a token
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<string> SignIn(UserDto user);
       /// <summary>
       /// validates a password matches that of a password in the database
       /// </summary>
       /// <param name="user"></param>
       /// <returns></returns>
        Task<bool> ValidatePassword(UserDto user);
        ApplicationUser Get(Guid id);
        Task<ApplicationUser> Get(string username);
        Task<ApplicationUser> Find(string email);
        Task<IdentityResult> ConfirmEmail(Guid id, string token);
        Task<string> GenerateConfirmationToken(Guid id);
        Task<IdentityResult> ResetPassword(UserResetDTO reset);
        Task<string> GeneratePasswordResetToken(Guid id);
        Task<IdentityResult> ChangePassword(UserChangePasswordDto details);
        Task<IdentityResult> DeleteAccount(ApplicationUser user);
    }
}