using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChiropracticApi.Data;
using ChiropracticApi.Models;
using ChiropracticApi.Dtos;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using BCrypt.Net;

namespace ChiropracticApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]  // Requiere autenticación para todas las acciones en este controlador
    public class UsersController : ControllerBase
    {
        private readonly ChiropracticContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ChiropracticContext context, IMapper mapper,ILogger<UsersController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _context.User.ToListAsync();
            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);
            return Ok(userDtos);
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id,[FromBody] CreateUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            _mapper.Map(userDto, user);


            if (!string.IsNullOrEmpty(userDto.Password))
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
            }
            user.Updated_At = DateTime.UtcNow;
            try
                {
                    _context.User.Update(user);
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "User updated successfully" });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating user");
                    return StatusCode(500, "Internal server error");
                }
            }
        // POST: api/Users
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> PostUser(CreateUserDto createUserDto)
        {
            var user = _mapper.Map<User>(createUserDto);

            // Cifrar la contraseña antes de guardar el usuario
            user.Password = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            var createdUserDto = _mapper.Map<UserDto>(user);

            return CreatedAtAction(nameof(GetUser), new { id = createdUserDto.IdUsuario }, createdUserDto);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.User
                .Include(u => u.UserAppointments)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (user == null)
            {
                return NotFound();
            }

            _context.User_Appointment.RemoveRange(user.UserAppointments);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.IdUsuario == id);
        }
    }
}
