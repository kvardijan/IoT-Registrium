using Microsoft.AspNetCore.Mvc;
using user_service.DTOs;
using user_service.Models;
using user_service.Services;

namespace user_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly TokenService _tokenService;
        public UserController(TokenService tokenService, UserService userService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequestDto request)
        { 
            var user = _userService.Authenticate(request.Username, request.Password);

            if (user == null)
                return Unauthorized("Invalid credentials");

            var token = _tokenService.CreateToken(user);

            return Ok(new { Message = "Login successful", Token = token });
        }
    }
}
