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
        Task<string> GenerateConfirmationToken(Guid id);
        ApplicationUser Get(Guid id);
        Task<ApplicationUser> Get(string username);
        Task<IdentityResult> ConfirmEmail(Guid id, string token);
        Task<ApplicationUser> Find(string email);
        Task<IdentityResult> ResetPassword(Guid id, string token, string newPassword);
    }
}