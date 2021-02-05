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
using UptimeAPI.Services.Email;

namespace UptimeAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    /// <summary>
    /// class for creating and authorizing users
    /// </summary>
    public class AuthController : ControllerBase
    {
        private readonly IApplicationUserRepository _userRepository;
        private readonly IEmailService _email;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _webHost;
        public AuthController(IApplicationUserRepository webUserRepository, IEmailService email,
            IWebHostEnvironment webHost,
            IConfiguration config)
        {
            _userRepository = webUserRepository;
            _email = email;
            _webHost = webHost;
            _config = config;
        }

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
                    _email.SendEmail(user.Email, url,EmailTemplates.ConfirmNewAccount);
                    return Ok();
                }
                return Conflict(userCreateResult.Errors.First().Description);
            }
            return Conflict("Email already exists");
        }
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

        [Authorize]
        [HttpPut("Profile/{id}")]
        public async Task<IActionResult> Update(Guid id, UserDto user)
        {

            return Ok();
        }

        [HttpGet("ForgotPassword/{email}")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
           ApplicationUser user = await  _userRepository.Find(email);

            if (user == null)
                BadRequest("email does not exist");
            string token = await _userRepository.GenerateConfirmationToken(user.Id);

            string hostname = _webHost.EnvironmentName.Equals("Development") ?
                                    _config.GetSection("Redirect")["HostnameDev"] :
                                    _config.GetSection("Redirect")["Hostname"];

            string path = _config.GetSection("Redirect")["ResetPassword"];

            string url = $"{hostname}{path}?id={user.Id}&email={user.Email}&token={token}";
            string encodedUrl = HttpUtility.UrlEncode(url);
            _email.SendEmail(email, encodedUrl, EmailTemplates.ResetPassword);
            return Ok();
        }

    }
}

