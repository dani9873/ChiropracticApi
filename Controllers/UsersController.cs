using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChiropracticApi.Data;
using ChiropracticApi.Models;
using ChiropracticApi.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace ChiropracticApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ChiropracticContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ChiropracticContext context, IMapper mapper, ILogger<UsersController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los usuarios.
        /// </summary>
        /// <param name="pageNumber">Número de la página.</param>
        /// <param name="pageSize">Tamaño de la página.</param>
        /// <returns>Lista de usuarios.</returns>
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Getting users - Page Number: {PageNumber}, Page Size: {PageSize}", pageNumber, pageSize);

            if (pageNumber <= 0 || pageSize <= 0)
            {
                _logger.LogWarning("Invalid page number or page size: Page Number = {PageNumber}, Page Size = {PageSize}", pageNumber, pageSize);
                return BadRequest(new { message = "Page number and page size must be greater than zero." });
            }

            try
            {
                var users = await _context.User
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} users", users.Count);

                var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);
                return Ok(userDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                return StatusCode(500, new { message = "Internal server error. Please try again later." });
            }
        }

        /// <summary>
        /// Obtiene un usuario por ID.
        /// </summary>
        /// <param name="id">ID del usuario.</param>
        /// <returns>Usuario encontrado.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            _logger.LogInformation("Getting user with ID: {Id}", id);

            if (id <= 0)
            {
                _logger.LogWarning("Invalid user ID: {Id}", id);
                return BadRequest(new { message = "User ID must be greater than zero." });
            }

            try
            {
                var user = await _context.User.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {Id} not found", id);
                    return NotFound(new { message = "User not found" });
                }

                var userDto = _mapper.Map<UserDto>(user);
                _logger.LogInformation("User with ID {Id} retrieved successfully", id);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID {Id}", id);
                return StatusCode(500, new { message = "Internal server error. Please try again later." });
            }
        }

        /// <summary>
        /// Actualiza un usuario.
        /// </summary>
        /// <param name="id">ID del usuario.</param>
        /// <param name="updateUserDto">Datos del usuario a actualizar.</param>
        /// <returns>Resultado de la actualización.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, [FromBody] CreateUserDto updateUserDto)
        {
            _logger.LogInformation("Updating user with ID: {Id}", id);

            if (id <= 0)
            {
                _logger.LogWarning("Invalid user ID: {Id}", id);
                return BadRequest(new { message = "User ID must be greater than zero." });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for user update with ID: {Id}", id);
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _context.User.FindAsync(id);
                if (user == null)
                {
                    _logger.LogWarning("User with ID {Id} not found for update", id);
                    return NotFound(new { message = "User not found" });
                }

                _mapper.Map(updateUserDto, user);

                if (!string.IsNullOrEmpty(updateUserDto.Password))
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(updateUserDto.Password);
                }

                user.Updated_At = DateTime.UtcNow;

                _context.User.Update(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User with ID {Id} updated successfully", id);
                return Ok(new { message = "User updated successfully" });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    _logger.LogWarning("User with ID {Id} not found during update", id);
                    return NotFound(new { message = "User not found" });
                }
                else
                {
                    _logger.LogError("Concurrency error updating user with ID {Id}", id);
                    return StatusCode(409, new { message = "Concurrency error. Please try again." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID {Id}", id);
                return StatusCode(500, new { message = "Internal server error. Please try again later." });
            }
        }

        /// <summary>
        /// Crea un nuevo usuario.
        /// </summary>
        /// <param name="createUserDto">Datos del nuevo usuario.</param>
        /// <returns>Usuario creado.</returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PostUser([FromBody] CreateUserDto createUserDto)
        {
            _logger.LogInformation("Creating a new user");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for user creation");
                return BadRequest(ModelState);
            }

            try
            {
                var user = _mapper.Map<User>(createUserDto);

                user.Password = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
                user.Role_idrole = 3;

                _context.User.Add(user);
                await _context.SaveChangesAsync();

                var createdUserDto = _mapper.Map<UserDto>(user);

                _logger.LogInformation("User created successfully with ID: {Id}", createdUserDto.IdUsuario);
                return CreatedAtAction(nameof(GetUser), new { id = createdUserDto.IdUsuario }, createdUserDto);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error creating user");
                return StatusCode(409, new { message = "Database conflict error. Please try again later." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return StatusCode(500, new { message = "Internal server error. Please try again later." });
            }
        }

        /// <summary>
        /// Elimina un usuario por ID.
        /// </summary>
        /// <param name="id">ID del usuario a eliminar.</param>
        /// <returns>Resultado de la eliminación.</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            _logger.LogInformation("Deleting user with ID: {Id}", id);

            if (id <= 0)
            {
                _logger.LogWarning("Invalid user ID: {Id}", id);
                return BadRequest(new { message = "User ID must be greater than zero." });
            }

            try
            {
                var user = await _context.User
                    .Include(u => u.UserAppointments)
                    .FirstOrDefaultAsync(u => u.IdUsuario == id);

                if (user == null)
                {
                    _logger.LogWarning("User with ID {Id} not found for deletion", id);
                    return NotFound(new { message = "User not found" });
                }

                _context.User_Appointment.RemoveRange(user.UserAppointments);
                _context.User.Remove(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User with ID {Id} deleted successfully", id);
                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error deleting user with ID {Id}", id);
                return StatusCode(409, new { message = "Database conflict error. Please try again later." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID {Id}", id);
                return StatusCode(500, new { message = "Internal server error. Please try again later." });
            }
        }

        private bool UserExists(int id)
        {
            try
            {
                _logger.LogInformation("Checking if user with ID {Id} exists", id);
                var exists = _context.User.Any(e => e.IdUsuario == id);
                _logger.LogInformation("User with ID {Id} exists: {Exists}", id, exists);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user with ID {Id} exists", id);
                return false;
            }
        }
    }
}
