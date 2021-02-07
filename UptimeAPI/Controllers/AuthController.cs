using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using UptimeAPI.Controllers.DTOs;
using UptimeAPI.Controllers.Repositories;
using UptimeAPI.Services;
using UptimeAPI.Services.Email;

namespace UptimeAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    /// <summary>
    /// class for creating and authorizing users
    /// </summary>
    public class AuthController : ControllerBase
    {
        private readonly IApplicationUserRepository _userRepository;
        private readonly IEmailService _email;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _webHost;
        private readonly IAuthorizationService _authorizationService;

        public AuthController(IApplicationUserRepository webUserRepository, IEmailService email,
            IWebHostEnvironment webHost, IAuthorizationService authorization,
            IConfiguration config)
        {
            _userRepository = webUserRepository;
            _email = email;
            _webHost = webHost;
            _config = config;
            _authorizationService = authorization;
        }
        [AllowAnonymous]
        [HttpPost("SignUp")]
        public async Task<ActionResult<string>> SignUp(UserDto userdto)
        {
            var exists = await _userRepository.Exists(userdto.Username);

            if (!exists)
            {
                IdentityResult userCreateResult = await _userRepository.PostAsync(userdto);
                if (userCreateResult.Succeeded)
                {
                    ApplicationUser user = await _userRepository.Get(userdto.Username);
                    string token = await _userRepository.GenerateConfirmationToken(user.Id);
                    string encodeToken = HttpUtility.UrlEncode(token);
                    string host = HttpContext.Request.Host.Value;
                    string protocol = HttpContext.Request.Scheme;
                    string url = $"{protocol}://{host}/api/Auth/{nameof(ConfirmEmail)}?id={user.Id}&token={encodeToken}";
                    _email.SendEmail(user.Email, url, EmailTemplates.ConfirmNewAccount);
                    return Ok();
                }
                return Conflict(userCreateResult.Errors.First().Description);
            }
            return Conflict("Email already exists");
        }
        [AllowAnonymous]
        [HttpGet("ConfirmEmail")]
        public async Task<RedirectResult> ConfirmEmail(Guid id, string token)
        {
            IdentityResult result = await _userRepository.ConfirmEmail(id, token);
            string hostname = _webHost.EnvironmentName.Equals("Development") ?
                                                _config.GetSection("Redirect")["HostnameDev"] :
                                                _config.GetSection("Redirect")["Hostname"];


            string path = result.Succeeded ?
                  _config.GetSection("Redirect")["EmailConfirmedSuccess"] :
                  _config.GetSection("Redirect")["EmailConfirmedFailure"];

            return Redirect(hostname + path);
        }
        [AllowAnonymous]
        [HttpPost("SignIn")]
        public async Task<ActionResult<string>> SignIn(UserDto userDTO)
        {
            bool exists = await _userRepository.Exists(userDTO.Username);
            if (!exists)
                return BadRequest("incorrect user name");

            bool passwordValid = await _userRepository.ValidatePassword(userDTO);
            if (!passwordValid)
                return BadRequest("username or password incorrect");


            return Ok(await _userRepository.SignIn(userDTO));

        }
        [AllowAnonymous]
        [HttpGet("ForgotPassword/{email}")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            ApplicationUser user = await _userRepository.Find(email);

            if (user == null)
                BadRequest("email does not exist");
            string token = await _userRepository.GeneratePasswordResetToken(user.Id);

            string hostname = _webHost.EnvironmentName.Equals("Development") ?
                                    _config.GetSection("Redirect")["HostnameDev"] :
                                    _config.GetSection("Redirect")["Hostname"];

            string path = _config.GetSection("Redirect")["ResetPassword"];
            string tokenEncoded = HttpUtility.UrlEncode(token);
            string url = $"{hostname}{path}?id={user.Id}&email={user.Email}&token={tokenEncoded}";

            _email.SendEmail(email, url, EmailTemplates.ResetPassword);
            return NoContent();
        }
        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(UserResetDTO reset)
        {
            IdentityResult result = await _userRepository.ResetPassword(reset);
            if (result.Succeeded)
                return Ok();
            return BadRequest(result.Errors.ToList()[0].Description);
        }

        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword(UserChangePasswordDto details)
        {
            ApplicationUser user = await _userRepository.Find(details.Username);
            if (user is null)
                BadRequest("Invalid email address");

            AuthorizationResult auth = await _authorizationService.AuthorizeAsync(User, user, Operations.Update);
            if (!auth.Succeeded)
                return new ForbidResult();

            IdentityResult result = await _userRepository.ChangePassword(details);
            if (result.Succeeded)
                return NoContent();
            return BadRequest(result.Errors.ToList()[0].Description);
        }

        [HttpDelete("DeleteAccount/{id}")]
        public async Task<IActionResult> DeleteAccount(Guid id)
        {
            ApplicationUser user =  _userRepository.Get(id);
            if (user is null)
                BadRequest("Invalid user");

            AuthorizationResult auth = await _authorizationService.AuthorizeAsync(User, user, Operations.Delete);
            if (!auth.Succeeded)
                return new ForbidResult();

            IdentityResult result = await _userRepository.DeleteAccount(user);
            if (result.Succeeded)
                return NoContent();
            return BadRequest(result.Errors.ToList()[0].Description);
        }
    }
}

