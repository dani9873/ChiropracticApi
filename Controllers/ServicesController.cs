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
    public class ServicesController : ControllerBase
    {
        private readonly ChiropracticContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ServicesController> _logger;

        public ServicesController(ChiropracticContext context, IMapper mapper, ILogger<ServicesController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los servicios.
        /// </summary>
        /// <returns>Lista de servicios.</returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetServices([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("Getting services - Page Number: {PageNumber}, Page Size: {PageSize}", pageNumber, pageSize);

            try
            {
                var services = await _context.Service
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} services", services.Count);

                var serviceDtos = _mapper.Map<IEnumerable<ServiceDto>>(services);
                return Ok(serviceDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving services");
                return StatusCode(500, new { message = "Internal server error. Please try again later." });
            }
        }

        /// <summary>
        /// Obtiene un servicio por ID.
        /// </summary>
        /// <param name="id">ID del servicio.</param>
        /// <returns>Servicio encontrado.</returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetService(int id)
        {
            _logger.LogInformation("Getting service with ID: {Id}", id);

            try
            {
                var service = await _context.Service.FindAsync(id);
                if (service == null)
                {
                    _logger.LogWarning("Service with ID {Id} not found", id);
                    return NotFound(new { message = "Service not found" });
                }

                var serviceDto = _mapper.Map<ServiceDto>(service);
                _logger.LogInformation("Service with ID {Id} retrieved successfully", id);
                return Ok(serviceDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving service with ID {Id}", id);
                return StatusCode(500, new { message = "Internal server error. Please try again later." });
            }
        }

        /// <summary>
        /// Actualiza un servicio.
        /// </summary>
        /// <param name="id">ID del servicio.</param>
        /// <param name="serviceDto">Datos del servicio a actualizar.</param>
        /// <returns>Resultado de la actualización.</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> PutService(int id, [FromBody] ServiceCreateDto serviceDto)
        {
            _logger.LogInformation("Updating service with ID: {Id}", id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for service update with ID: {Id}", id);
                return BadRequest(ModelState);
            }

            try
            {
                var service = await _context.Service.FindAsync(id);
                if (service == null)
                {
                    _logger.LogWarning("Service with ID {Id} not found for update", id);
                    return NotFound(new { message = "Service not found" });
                }

                _mapper.Map(serviceDto, service);
                _context.Entry(service).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Service with ID {Id} updated successfully", id);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(id))
                {
                    _logger.LogWarning("Service with ID {Id} not found during concurrency check", id);
                    return NotFound(new { message = "Service not found" });
                }
                else
                {
                    _logger.LogError("Concurrency error updating service with ID {Id}", id);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating service with ID {Id}", id);
                return StatusCode(500, new { message = "Internal server error. Please try again later." });
            }
        }

        /// <summary>
        /// Crea un nuevo servicio.
        /// </summary>
        /// <param name="serviceDto">Datos del nuevo servicio.</param>
        /// <returns>Servicio creado.</returns>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> PostService([FromBody] ServiceCreateDto serviceDto)
        {
            _logger.LogInformation("Creating a new service");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for service creation");
                return BadRequest(ModelState);
            }

            try
            {
                var service = _mapper.Map<Service>(serviceDto);

                _context.Service.Add(service);
                await _context.SaveChangesAsync();

                var createdServiceDto = _mapper.Map<ServiceDto>(service);

                _logger.LogInformation("Service created successfully with ID: {Id}", createdServiceDto.IdService);
                return CreatedAtAction(nameof(GetService), new { id = createdServiceDto.IdService }, createdServiceDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating service");
                return StatusCode(500, new { message = "Internal server error. Please try again later." });
            }
        }

        /// <summary>
        /// Elimina un servicio por ID.
        /// </summary>
        /// <param name="id">ID del servicio a eliminar.</param>
        /// <returns>Resultado de la eliminación.</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteService(int id)
        {
            _logger.LogInformation("Deleting service with ID: {Id}", id);

            try
            {
                var service = await _context.Service.FindAsync(id);
                if (service == null)
                {
                    _logger.LogWarning("Service with ID {Id} not found for deletion", id);
                    return NotFound(new { message = "Service not found" });
                }

                _context.Service.Remove(service);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Service with ID {Id} deleted successfully", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting service with ID {Id}", id);
                return StatusCode(500, new { message = "Internal server error. Please try again later." });
            }
        }

        private bool ServiceExists(int id)
        {
            try
            {
                _logger.LogInformation("Checking if service with ID {Id} exists", id);
                var exists = _context.Service.Any(e => e.IdService == id);
                _logger.LogInformation("Service with ID {Id} exists: {Exists}", id, exists);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if service with ID {Id} exists", id);
                return false;
            }
        }
    }
}
