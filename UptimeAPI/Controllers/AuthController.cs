using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Data.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace UptimeAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;

        public AuthController(UserManager<IdentityUser> userManager, IMapper mapper)
        {
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet("")]
        public async Task<IActionResult> Username()
        {

            return Problem("Username already exists");
        }
        [HttpPost("")]
        public async Task<IActionResult> SignUp(UserDTO userDTO)
        {
            var userManager = await _userManager.FindByNameAsync(userDTO.Username);
            if (Object.Equals(userManager,null))
            {
                var user = _mapper.Map<UserDTO,IdentityUser>(userDTO);
                var userCreateResult = await _userManager.CreateAsync(user, userDTO.Password);
                if (userCreateResult.Succeeded)
                {
                    return Created(String.Empty, String.Empty);
                }
                return Problem(userCreateResult.Errors.First().Description, null, 500);
            }

            return Conflict("Username already exists");
        }

    }
}

