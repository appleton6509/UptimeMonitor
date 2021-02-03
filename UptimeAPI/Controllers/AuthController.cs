using System;
using System.Collections.Generic;
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
        private readonly IApplicationUserRepository _userRepository;

        public AuthController( IApplicationUserRepository webUserRepository)
        {
            _userRepository = webUserRepository;
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(UserDto userdto)
        {
            var exists = await _userRepository.Exists(userdto.Username);

            if (!exists)
            {
                var userCreateResult = await _userRepository.PostAsync(userdto);
                if (userCreateResult.Succeeded)
                {
                    return Created(String.Empty,String.Empty);
                }
                return Conflict(userCreateResult.Errors.First().Description);
            }
            return Conflict("Email already exists");
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(UserDto userDTO)
        {
            bool exists = await _userRepository.Exists(userDTO.Username);

            if (!exists)
                return BadRequest("incorrect user name");

            var passwordValid = await _userRepository.ValidatePassword(userDTO);

            if (passwordValid)
                return Ok(await _userRepository.SignIn(userDTO));
            return BadRequest("username or password incorrect");
        }

        [Authorize]
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(Guid id, UserDto user)
        {

            return Ok();
        }

    }
}

