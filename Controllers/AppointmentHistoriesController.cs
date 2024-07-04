using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChiropracticApi.Data;
using ChiropracticApi.Models;
using ChiropracticApi.Dtos;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace ChiropracticApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentHistoriesController : ControllerBase
    {
        private readonly ChiropracticContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AppointmentHistoriesController> _logger;

        public AppointmentHistoriesController(ChiropracticContext context, IMapper mapper, ILogger<AppointmentHistoriesController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Gets all appointment histories.
        /// </summary>
        /// <returns>List of AppointmentHistoryDto</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentHistoryDto>>> GetAppointmentHistories()
        {
            _logger.LogInformation("Fetching all appointment histories");

            var appointmentHistories = await _context.appointment_history.ToListAsync();
            var appointmentHistoryDtos = _mapper.Map<IEnumerable<AppointmentHistoryDto>>(appointmentHistories);

            _logger.LogInformation("Fetched {Count} appointment histories", appointmentHistoryDtos.Count());

            return Ok(appointmentHistoryDtos);
        }

        /// <summary>
        /// Gets an appointment history by id.
        /// </summary>
        /// <param name="id">Appointment history id</param>
        /// <returns>AppointmentHistoryDto</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentHistoryDto>> GetAppointmentHistory(int id)
        {
            _logger.LogInformation("Fetching appointment history with id {Id}", id);

            var appointmentHistory = await _context.appointment_history
                .FirstOrDefaultAsync(ah => ah.Idappointment_history == id);

            if (appointmentHistory == null)
            {
                _logger.LogWarning("Appointment history with id {Id} not found", id);
                return NotFound();
            }

            var appointmentHistoryDto = _mapper.Map<AppointmentHistoryDto>(appointmentHistory);
            _logger.LogInformation("Fetched appointment history with id {Id}", id);

            return Ok(appointmentHistoryDto);
        }

        /// <summary>
        /// Updates an appointment history.
        /// </summary>
        /// <param name="id">Appointment history id</param>
        /// <param name="appointmentHistoryDto">Appointment history data</param>
        /// <returns>NoContent if successful</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointmentHistory(int id, AppointmentHistoryCreateDto appointmentHistoryDto)
        {
            _logger.LogInformation("Updating appointment history with id {Id}", id);

            var appointmentHistory = await _context.appointment_history.FindAsync(id);
            if (appointmentHistory == null)
            {
                _logger.LogWarning("Appointment history with id {Id} not found", id);
                return NotFound();
            }

            _mapper.Map(appointmentHistoryDto, appointmentHistory);
            _context.Entry(appointmentHistory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Updated appointment history with id {Id}", id);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!AppointmentHistoryExists(id))
                {
                    _logger.LogWarning("Appointment history with id {Id} no longer exists", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Error updating appointment history with id {Id}", id);
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// Creates a new appointment history.
        /// </summary>
        /// <param name="appointmentHistoryDto">Appointment history data</param>
        /// <returns>Created AppointmentHistoryDto with ID</returns>
        [HttpPost]
        public async Task<ActionResult<AppointmentHistoryDto>> PostAppointmentHistory(AppointmentHistoryCreateDto appointmentHistoryDto)
        {
            _logger.LogInformation("Creating new appointment history");

            var appointmentHistory = _mapper.Map<AppointmentHistory>(appointmentHistoryDto);
            _context.appointment_history.Add(appointmentHistory);
            await _context.SaveChangesAsync();

            var createdAppointmentHistoryDto = _mapper.Map<AppointmentHistoryDto>(appointmentHistory);

            _logger.LogInformation("Created new appointment history with id {Id}", createdAppointmentHistoryDto.IdAppointmentHistory);

            return CreatedAtAction(nameof(GetAppointmentHistory), new { id = createdAppointmentHistoryDto.IdAppointmentHistory }, createdAppointmentHistoryDto);
        }

        /// <summary>
        /// Deletes an appointment history by id.
        /// </summary>
        /// <param name="id">Appointment history id</param>
        /// <returns>NoContent if successful</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointmentHistory(int id)
        {
            _logger.LogInformation("Deleting appointment history with id {Id}", id);

            var appointmentHistory = await _context.appointment_history.FindAsync(id);
            if (appointmentHistory == null)
            {
                _logger.LogWarning("Appointment history with id {Id} not found", id);
                return NotFound();
            }

            _context.appointment_history.Remove(appointmentHistory);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted appointment history with id {Id}", id);

            return NoContent();
        }

        private bool AppointmentHistoryExists(int id)
        {
            try
            {
                _logger.LogInformation("Checking if appointment history with id {Id} exists", id);
                return _context.appointment_history.Any(e => e.Idappointment_history == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if appointment history with id {Id} exists", id);
                throw;
            }
        }
    }
}
