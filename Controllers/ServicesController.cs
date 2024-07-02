using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChiropracticApi.Data;
using ChiropracticApi.Models;
using ChiropracticApi.Dtos;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChiropracticApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        private readonly ChiropracticContext _context;
        private readonly IMapper _mapper;

        public ServicesController(ChiropracticContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Services
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceDto>>> GetServices()
        {
            var services = await _context.Service.ToListAsync();
            var serviceDtos = _mapper.Map<IEnumerable<ServiceDto>>(services);
            return Ok(serviceDtos);
        }

        // GET: api/Services/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceDto>> GetService(int id)
        {
            var service = await _context.Service.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            var serviceDto = _mapper.Map<ServiceDto>(service);
            return Ok(serviceDto);
        }

       // PUT: api/Services
        [HttpPut]
        public async Task<IActionResult> PutService(int id, ServiceCreateDto serviceDto)
        {
            var service = await _context.Service.FindAsync(id);

            if (service == null)
            {
                return NotFound();
            }

            _mapper.Map(serviceDto, service);
            _context.Entry(service).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(id))
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

        // POST: api/Services
        [HttpPost]
        public async Task<ActionResult<ServiceDto>> PostService(ServiceCreateDto serviceDto)
        {
            var service = _mapper.Map<Service>(serviceDto);

            _context.Service.Add(service);
            await _context.SaveChangesAsync();

            var createdServiceDto = _mapper.Map<ServiceDto>(service);

            return CreatedAtAction(nameof(GetService), new { id = createdServiceDto.IdService }, createdServiceDto);
        }

        // DELETE: api/Services/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var service = await _context.Service.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            _context.Service.Remove(service);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServiceExists(int id)
        {
            return _context.Service.Any(e => e.IdService == id);
        }
    }
}
