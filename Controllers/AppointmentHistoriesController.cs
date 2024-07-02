using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChiropracticApi.Data;
using ChiropracticApi.Models;
using ChiropracticApi.Dtos;
using AutoMapper;

namespace ChiropracticApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentHistoriesController : ControllerBase
    {
        private readonly ChiropracticContext _context;
        private readonly IMapper _mapper;

        public AppointmentHistoriesController(ChiropracticContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/appointment_history
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentHistory>>> GetAppointmentHistories()
        {
            var appointmentHistories = await _context.appointment_history.ToListAsync();
            var appointmentHistoryDtos = _mapper.Map<IEnumerable<AppointmentHistory>>(appointmentHistories);
            return Ok(appointmentHistoryDtos);
        }

        // GET: api/appointment_history/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentHistory>> GetAppointmentHistory(int id)
        {
            var appointmentHistory = await _context.appointment_history
                .FirstOrDefaultAsync(ah => ah.Idappointment_history == id);

            if (appointmentHistory == null)
            {
                return NotFound();
            }

            var appointmentHistoryDto = _mapper.Map<AppointmentHistoryDto>(appointmentHistory);
            return Ok(appointmentHistoryDto);
        }

        // PUT: api/appointment_history/5
       // PUT: api/AppointmentHistories
        [HttpPut]
        public async Task<IActionResult> PutAppointmentHistory(int id, AppointmentHistoryCreateDto appointmentHistoryDto)
        {
            var appointmentHistory = await _context.appointment_history.FindAsync(id);

            if (appointmentHistory == null)
            {
                return NotFound();
            }

            _mapper.Map(appointmentHistoryDto, appointmentHistory);
            _context.Entry(appointmentHistory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentHistoryExists(id))
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

        // POST: api/appointment_history
        [HttpPost]
        public async Task<ActionResult<AppointmentHistoryDto>> PostAppointmentHistory(AppointmentHistoryCreateDto appointmentHistoryDto)
        {
            var appointmentHistory = _mapper.Map<AppointmentHistory>(appointmentHistoryDto);

            _context.appointment_history.Add(appointmentHistory);
            await _context.SaveChangesAsync();

            var createdAppointmentHistoryDto = _mapper.Map<AppointmentHistoryDto>(appointmentHistory);

            return CreatedAtAction(nameof(GetAppointmentHistory), new { id = createdAppointmentHistoryDto.IdAppointmentHistory }, createdAppointmentHistoryDto);
        }

        // DELETE: api/appointment_history/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointmentHistory(int id)
        {
            var appointmentHistory = await _context.appointment_history.FindAsync(id);
            if (appointmentHistory == null)
            {
                return NotFound();
            }

            _context.appointment_history.Remove(appointmentHistory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AppointmentHistoryExists(int id)
        {
            return _context.appointment_history.Any(e => e.Idappointment_history == id);
        }
    }
}
