using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Data;
using Data.DTOs;
using Data.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UptimeAPI.Services;
using UptimeAPI.Settings;

namespace UptimeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;
        private readonly UptimeContext _context;

        public AuthController(
            UserManager<IdentityUser> userManager
            , IMapper mapper
            , IOptionsSnapshot<JwtSettings> jwtSettings
            , UptimeContext context)
        {
            _mapper = mapper;
            _userManager = userManager;
            _jwtSettings = jwtSettings.Value;
            _context = context;
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
                    _context.WebUser.Add(new WebUser(userDTO.Username,user.Id));
                    await  _context.SaveChangesAsync();
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
            WebUser webUser = _context.WebUser.FirstOrDefault(x => x.IdentityId.Equals(user.Id));
            if (passwordIsValid && !Object.Equals(webUser, null))
                    return Ok(new JwtTokenService().GenerateToken(webUser, _jwtSettings));
            return BadRequest("username or password incorrect");
        }

    }
}

