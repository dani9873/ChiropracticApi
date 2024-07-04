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
using Microsoft.Extensions.Logging;
using System;

namespace ChiropracticApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly ChiropracticContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(ChiropracticContext context, IMapper mapper, ILogger<AppointmentsController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Gets all appointments.
        /// </summary>
        /// <returns>List of AppointmentDto</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointments()
        {
            _logger.LogInformation("Fetching all appointments");

            var appointments = await _context.Appointment
                .Include(a => a.UserAppointments)
                    .ThenInclude(ua => ua.User)
                .Include(a => a.UserAppointments)
                    .ThenInclude(ua => ua.Service)
                .Include(a => a.appointment_history)
                .ToListAsync();

            var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
            _logger.LogInformation("Fetched {Count} appointments", appointmentDtos.Count());

            return Ok(appointmentDtos);
        }

        /// <summary>
        /// Gets an appointment by id.
        /// </summary>
        /// <param name="id">Appointment id</param>
        /// <returns>AppointmentDto</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDto>> GetAppointment(int id)
        {
            _logger.LogInformation("Fetching appointment with id {Id}", id);

            var appointment = await _context.Appointment
                .Include(a => a.UserAppointments)
                    .ThenInclude(ua => ua.User)
                .Include(a => a.UserAppointments)
                    .ThenInclude(ua => ua.Service)
                .Include(a => a.appointment_history)
                .FirstOrDefaultAsync(a => a.IdAppointment == id);

            if (appointment == null)
            {
                _logger.LogWarning("Appointment with id {Id} not found", id);
                return NotFound();
            }

            var appointmentDto = _mapper.Map<AppointmentDto>(appointment);
            _logger.LogInformation("Fetched appointment with id {Id}", id);

            return Ok(appointmentDto);
        }

        /// <summary>
        /// Updates an appointment.
        /// </summary>
        /// <param name="id">Appointment id</param>
        /// <param name="appointmentDto">Appointment data</param>
        /// <returns>NoContent if successful</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointment(int id, AppointmentCreateDto appointmentDto)
        {
            _logger.LogInformation("Updating appointment with id {Id}", id);

            var appointment = await _context.Appointment.FindAsync(id);
            if (appointment == null)
            {
                _logger.LogWarning("Appointment with id {Id} not found", id);
                return NotFound();
            }

            _mapper.Map(appointmentDto, appointment);
            if (appointment.Canceled_At == DateTime.MinValue)
            {
                appointment.Canceled_At = null;
            }

            _context.Entry(appointment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated appointment with id {Id}", id);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!AppointmentExists(id))
                {
                    _logger.LogWarning("Appointment with id {Id} no longer exists", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Error updating appointment with id {Id}", id);
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Creates a new appointment.
        /// </summary>
        /// <param name="appointmentDto">Appointment data</param>
        /// <returns>Created AppointmentDto with ID</returns>
        [HttpPost]
        public async Task<ActionResult<AppointmentDto>> PostAppointment(AppointmentCreateDto appointmentDto)
        {
            _logger.LogInformation("Creating new appointment");

            var appointment = _mapper.Map<Appointment>(appointmentDto);
            if (appointment.Canceled_At == DateTime.MinValue)
            {
                appointment.Canceled_At = null;
            }

            _context.Appointment.Add(appointment);
            await _context.SaveChangesAsync();

            var createdAppointmentDto = _mapper.Map<AppointmentDto>(appointment);

            _logger.LogInformation("Created new appointment with id {Id}", createdAppointmentDto.IdAppointment);

            return CreatedAtAction(nameof(GetAppointment), new { id = createdAppointmentDto.IdAppointment }, createdAppointmentDto);
        }

        /// <summary>
        /// Deletes an appointment by id.
        /// </summary>
        /// <param name="id">Appointment id</param>
        /// <returns>NoContent if successful</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            _logger.LogInformation("Deleting appointment with id {Id}", id);

            var appointment = await _context.Appointment
                .Include(a => a.appointment_history)
                .FirstOrDefaultAsync(a => a.IdAppointment == id);

            if (appointment == null)
            {
                _logger.LogWarning("Appointment with id {Id} not found", id);
                return NotFound();
            }

            _context.appointment_history.RemoveRange(appointment.appointment_history);
            _context.Appointment.Remove(appointment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted appointment with id {Id}", id);

            return NoContent();
        }

        private bool AppointmentExists(int id)
        {
            try
            {
                _logger.LogInformation("Checking if appointment with id {Id} exists", id);
                return _context.Appointment.Any(e => e.IdAppointment == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if appointment with id {Id} exists", id);
                throw;
            }
        }
    }
}
