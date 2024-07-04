using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ChiropracticApi.Data;
using ChiropracticApi.Models;
using ChiropracticApi.Dtos;
using AutoMapper;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;

namespace ChiropracticApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ChiropracticContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ChiropracticContext context, IMapper mapper, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="userDto">User details.</param>
        /// <returns>Result of the registration.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCreateDto userDto)
        {
            _logger.LogInformation("Registering new user");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for user registration");
                return BadRequest(ModelState);
            }

            if (await UserExists(userDto.Email))
            {
                _logger.LogWarning("Email {Email} is already taken", userDto.Email);
                return BadRequest(new { message = "Email is already taken" });
            }

            var user = _mapper.Map<User>(userDto);
            user.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            user.Role_idrole = 2; // Assuming 2 is the default role for new users

            try
            {
                _context.User.Add(user);
                await _context.SaveChangesAsync();
                _logger.LogInformation("User registered successfully with email: {Email}", userDto.Email);
                return Ok(new { message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user with email: {Email}", userDto.Email);
                return StatusCode(500, new { message = "Internal server error. Please try again later." });
            }
        }

        /// <summary>
        /// Authenticates a user and generates a JWT token.
        /// </summary>
        /// <param name="userDto">User login details.</param>
        /// <returns>JWT token if successful.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login userDto)
        {
            _logger.LogInformation("User login attempt for email: {Email}", userDto.Email);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for user login");
                return BadRequest(ModelState);
            }

            var user = await _context.User.SingleOrDefaultAsync(u => u.Email == userDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(userDto.Password, user.Password))
            {
                _logger.LogWarning("Invalid credentials for email: {Email}", userDto.Email);
                return Unauthorized(new { message = "Invalid credentials" });
            }

            var token = GenerateJwtToken(user);
            _logger.LogInformation("User logged in successfully with email: {Email}", userDto.Email);
            return Ok(new { token });
        }

        private string GenerateJwtToken(User user)
{
    if (user == null) throw new ArgumentNullException(nameof(user));

    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role_idrole.ToString()),
    };

    var key = _configuration["Jwt:Key"];
    var issuer = _configuration["Jwt:Issuer"];
    var audience = _configuration["Jwt:Audience"];

    if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
    {
        throw new InvalidOperationException("JWT settings are not configured properly.");
    }

    var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
    var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

    var token = new JwtSecurityToken(
        issuer: issuer,
        audience: audience,
        claims: claims,
        expires: DateTime.UtcNow.AddDays(1),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}
        private async Task<bool> UserExists(string email)
        {
            try
            {
                _logger.LogInformation("Checking if user exists with email: {Email}", email);
                var exists = await _context.User.AnyAsync(u => u.Email == email);
                _logger.LogInformation("User exists with email: {Email}: {Exists}", email, exists);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user exists with email: {Email}", email);
                throw;
            }
        }
    }
}
