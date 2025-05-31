using Microsoft.AspNetCore.Mvc;
using StockPulse.Application.DTOs;
using StockPulse.Application.Interfaces;
using StockPulse.Infrastructure.Services;

namespace StockPulse.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthController(IUserService userService, IJwtTokenGenerator jwtTokenGenerator)
        {
            _userService = userService;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var user = await _userService.AuthenticateAsync(request.Username, request.Password);
            if (user == null)
                return Unauthorized("Invalid credentials");

            // Generate and return JWT or cookie-based auth here
            var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Username);
            return Ok(new { token });
        }
    }

}
