using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UptimeAPI.Controllers.DTOs;
using UptimeAPI.Controllers.Repositories;
using UptimeAPI.Services;
using UptimeAPI.Settings;

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
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;
        private readonly IWebUserRepository _webUserRepository;

        public AuthController(
            UserManager<IdentityUser> userManager
            , IMapper mapper
            , IOptionsSnapshot<JwtSettings> jwtSettings
           , IWebUserRepository webUserRepository)
        {
            _mapper = mapper;
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _webUserRepository = webUserRepository;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(WebUserDTO userDTO)
        {
            var userManager = await _userManager.FindByNameAsync(userDTO.Username);
            if (Object.Equals(userManager,null))
            {
                var user = _mapper.Map<WebUserDTO,IdentityUser>(userDTO);
                var userCreateResult = await _userManager.CreateAsync(user, userDTO.Password);
                if (userCreateResult.Succeeded)
                {
                    await _webUserRepository.PostAsync(new WebUser(userDTO.Username,user.Id));
                    return Created(String.Empty, String.Empty);
                }
 
                return Conflict(userCreateResult.Errors.First().Description);
            }
            return Conflict("Username already exists");
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(WebUserDTO userDTO)
        {
            IdentityUser user = await  _userManager.FindByNameAsync(userDTO.Username);

            if (Object.Equals(user,null))
                return BadRequest("incorrect user name");

            var passwordIsValid = await  _userManager.CheckPasswordAsync(user, userDTO.Password);
            WebUser webUser = _webUserRepository.Get(user.Id);
            if (passwordIsValid && !Object.Equals(webUser, null))
                    return Ok(new JwtTokenService().GenerateToken(webUser, _jwtSettings));
            return BadRequest("username or password incorrect");
        }

    }
}

