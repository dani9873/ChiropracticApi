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
    public class User_AppointmentController : ControllerBase
    {
        private readonly ChiropracticContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<User_AppointmentController> _logger;

        public User_AppointmentController(ChiropracticContext context, IMapper mapper, ILogger<User_AppointmentController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las citas de usuarios.
        /// </summary>
        /// <param name="pageNumber">Número de la página.</param>
        /// <param name="pageSize">Tamaño de la página.</param>
        /// <returns>Lista de citas de usuarios.</returns>
        [HttpGet]
        public async Task<IActionResult> GetUser_Appointment([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Getting user appointments - Page Number: {PageNumber}, Page Size: {PageSize}", pageNumber, pageSize);

            if (pageNumber <= 0 || pageSize <= 0)
            {
                _logger.LogWarning("Invalid page number or page size: Page Number = {PageNumber}, Page Size = {PageSize}", pageNumber, pageSize);
                return BadRequest(new { message = "Page number and page size must be greater than zero." });
            }

            try
            {
                var User_Appointment = await _context.User_Appointment
                    .Include(ua => ua.User)
                    .Include(ua => ua.Appointment)
                    .Include(ua => ua.Service)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} user appointments", User_Appointment.Count);

                var userAppointments = _mapper.Map<IEnumerable<UserAppointment>>(User_Appointment);
                return Ok(User_Appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user appointments");
                return StatusCode(500, new { message = "Internal server error. Please try again later." });
            }
        }

        /// <summary>
        /// Obtiene una cita de usuario por ID.
        /// </summary>
        /// <param name="id">ID de la cita de usuario.</param>
        /// <returns>Cita de usuario encontrada.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserAppointment(int id)
        {
            _logger.LogInformation("Getting user appointment with ID: {Id}", id);

            if (id <= 0)
            {
                _logger.LogWarning("Invalid user appointment ID: {Id}", id);
                return BadRequest(new { message = "User appointment ID must be greater than zero." });
            }

            try
            {
                var userAppointment = await _context.User_Appointment
                    .Include(ua => ua.User)
                    .Include(ua => ua.Appointment)
                    .Include(ua => ua.Service)
                    .FirstOrDefaultAsync(ua => ua.IdUser_Appointment == id);

                if (userAppointment == null)
                {
                    _logger.LogWarning("User appointment with ID {Id} not found", id);
                    return NotFound(new { message = "User appointment not found" });
                }

                var UserAppointment = _mapper.Map<UserAppointment>(userAppointment);
                _logger.LogInformation("User appointment with ID {Id} retrieved successfully", id);
                return Ok(UserAppointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user appointment with ID {Id}", id);
                return StatusCode(500, new { message = "Internal server error. Please try again later." });
            }
        }

        /// <summary>
        /// Actualiza una cita de usuario.
        /// </summary>
        /// <param name="id">ID de la cita de usuario.</param>
        /// <param name="UserAppointment">Datos de la cita de usuario a actualizar.</param>
        /// <returns>Resultado de la actualización.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserAppointment(int id, [FromBody] UserAppointment UserAppointment)
        {
            _logger.LogInformation("Updating user appointment with ID: {Id}", id);

            if (id <= 0)
            {
                _logger.LogWarning("Invalid user appointment ID: {Id}", id);
                return BadRequest(new { message = "User appointment ID must be greater than zero." });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for user appointment update with ID: {Id}", id);
                return BadRequest(ModelState);
            }

            try
            {
                var userAppointment = await _context.User_Appointment.FindAsync(id);
                if (userAppointment == null)
                {
                    _logger.LogWarning("User appointment with ID {Id} not found for update", id);
                    return NotFound(new { message = "User appointment not found" });
                }

                _mapper.Map(UserAppointment, userAppointment);

                userAppointment.Updated_At = DateTime.UtcNow;

                _context.User_Appointment.Update(userAppointment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User appointment with ID {Id} updated successfully", id);
                return Ok(new { message = "User appointment updated successfully" });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserAppointmentExists(id))
                {
                    _logger.LogWarning("User appointment with ID {Id} not found during update", id);
                    return NotFound(new { message = "User appointment not found" });
                }
                else
                {
                    _logger.LogError("Concurrency error updating user appointment with ID {Id}", id);
                    return StatusCode(409, new { message = "Concurrency error. Please try again." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user appointment with ID {Id}", id);
                return StatusCode(500, new { message = "Internal server error. Please try again later." });
            }
        }

        /// <summary>
        /// Crea una nueva cita de usuario.
        /// </summary>
        /// <param name="UserAppointment">Datos de la nueva cita de usuario.</param>
        /// <returns>Cita de usuario creada.</returns>
        [HttpPost]
        public async Task<IActionResult> PostUserAppointment([FromBody] UserAppointment UserAppointment)
        {
            _logger.LogInformation("Creating a new user appointment");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for user appointment creation");
                return BadRequest(ModelState);
            }

            try
            {
                var userAppointment = _mapper.Map<UserAppointment>(UserAppointment);

                _context.User_Appointment.Add(userAppointment);
                await _context.SaveChangesAsync();

                var createdUserAppointment = _mapper.Map<UserAppointment>(userAppointment);

                _logger.LogInformation("User appointment created successfully with ID: {Id}", createdUserAppointment.IdUser_Appointment);
                return CreatedAtAction(nameof(GetUserAppointment), new { id = createdUserAppointment.IdUser_Appointment }, createdUserAppointment);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error creating user appointment");
                return StatusCode(409, new { message = "Database conflict error. Please try again later." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user appointment");
                return StatusCode(500, new { message = "Internal server error. Please try again later." });
            }
        }

        /// <summary>
        /// Elimina una cita de usuario por ID.
        /// </summary>
        /// <param name="id">ID de la cita de usuario a eliminar.</param>
        /// <returns>Resultado de la eliminación.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAppointment(int id)
        {
            _logger.LogInformation("Deleting user appointment with ID: {Id}", id);

            if (id <= 0)
            {
                _logger.LogWarning("Invalid user appointment ID: {Id}", id);
                return BadRequest(new { message = "User appointment ID must be greater than zero." });
            }

            try
            {
                var userAppointment = await _context.User_Appointment.FindAsync(id);
                if (userAppointment == null)
                {
                    _logger.LogWarning("User appointment with ID {Id} not found for deletion", id);
                    return NotFound(new { message = "User appointment not found" });
                }

                _context.User_Appointment.Remove(userAppointment);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User appointment with ID {Id} deleted successfully", id);
                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database error deleting user appointment with ID {Id}", id);
                return StatusCode(409, new { message = "Database conflict error. Please try again later." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user appointment with ID {Id}", id);
                return StatusCode(500, new { message = "Internal server error. Please try again later." });
            }
        }

        private bool UserAppointmentExists(int id)
        {
            try
            {
                _logger.LogInformation("Checking if user appointment with ID {Id} exists", id);
                var exists = _context.User_Appointment.Any(e => e.IdUser_Appointment == id);
                _logger.LogInformation("User appointment with ID {Id} exists: {Exists}", id, exists);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user appointment with ID {Id} exists", id);
                return false;
            }
        }
    }
}
