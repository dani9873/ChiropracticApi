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

namespace ChiropracticApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Aplica el atributo [Authorize] a todo el controlador
    public class AppointmentsController : ControllerBase
    {
        private readonly ChiropracticContext _context;
        private readonly IMapper _mapper;

        public AppointmentsController(ChiropracticContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Appointments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointments()
        {
            var appointments = await _context.Appointment
                .Include(a => a.UserAppointments)
                    .ThenInclude(ua => ua.User)
                .Include(a => a.UserAppointments)
                    .ThenInclude(ua => ua.Service)
                .Include(a => a.appointment_history)
                .ToListAsync();

            var appointmentDtos = _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
            return Ok(appointmentDtos);
        }

        // GET: api/Appointments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDto>> GetAppointment(int id)
        {
            var appointment = await _context.Appointment
                .Include(a => a.UserAppointments)
                    .ThenInclude(ua => ua.User)
                .Include(a => a.UserAppointments)
                    .ThenInclude(ua => ua.Service)
                .Include(a => a.appointment_history)
                .FirstOrDefaultAsync(a => a.IdAppointment == id);

            if (appointment == null)
            {
                return NotFound();
            }

            var appointmentDto = _mapper.Map<AppointmentDto>(appointment);
            return Ok(appointmentDto);
        }

        // PUT: api/Appointments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointment(int id, AppointmentCreateDto appointmentDto)
        {
            var appointment = await _context.Appointment.FindAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            _mapper.Map(appointmentDto, appointment);
            if (appointment.Canceled_At == DateTime.MinValue)
            {
                appointment.Canceled_At = null; // o cualquier otra lógica de negocio
            }
            _context.Entry(appointment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Appointments
        [HttpPost]
        public async Task<ActionResult<AppointmentDto>> PostAppointment(AppointmentCreateDto appointmentDto)
        {
            var appointment = _mapper.Map<Appointment>(appointmentDto);

            if (appointment.Canceled_At == DateTime.MinValue)
            {
                appointment.Canceled_At = null; // o cualquier otra lógica de negocio
            }

            _context.Appointment.Add(appointment);
            await _context.SaveChangesAsync();

            var createdAppointmentDto = _mapper.Map<AppointmentDto>(appointment);

            return CreatedAtAction(nameof(GetAppointment), new { id = createdAppointmentDto.IdAppointment }, createdAppointmentDto);
        }

        // DELETE: api/Appointments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointment
                .Include(a => a.appointment_history)
                .FirstOrDefaultAsync(a => a.IdAppointment == id);
            if (appointment == null)
            {
                return NotFound();
            }
            _context.appointment_history.RemoveRange(appointment.appointment_history);


            _context.Appointment.Remove(appointment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointment.Any(e => e.IdAppointment == id);
        }
    }
}
