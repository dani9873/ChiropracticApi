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
using Microsoft.AspNetCore.Authorization;

namespace ChiropracticApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly ChiropracticContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<RoleController> _logger;

        public RoleController(ChiropracticContext context, IMapper mapper, ILogger<RoleController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los roles.
        /// </summary>
        /// <returns>Lista de RoleDto.</returns>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetRoles()
        {
            _logger.LogInformation("Getting all roles");

            try
            {
                var roles = await _context.Role.ToListAsync();
                _logger.LogInformation("Retrieved {Count} roles", roles.Count);

                var roleDtos = _mapper.Map<IEnumerable<RoleDto>>(roles);
                return Ok(roleDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving roles");
                return StatusCode(500, new { message = "Internal server error. Please try again later." });
            }
        }

        /// <summary>
        /// Obtiene un rol por ID.
        /// </summary>
        /// <param name="id">ID del rol.</param>
        /// <returns>RoleDto.</returns>
        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetRole(int id)
        {
            _logger.LogInformation("Getting role with ID: {Id}", id);

            try
            {
                var role = await _context.Role.FindAsync(id);
                if (role == null)
                {
                    _logger.LogWarning("Role with ID {Id} not found", id);
                    return NotFound(new { message = "Role not found" });
                }

                var roleDto = _mapper.Map<RoleDto>(role);
                _logger.LogInformation("Role with ID {Id} retrieved successfully", id);
                return Ok(roleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving role with ID {Id}", id);
                return StatusCode(500, new { message = "Internal server error. Please try again later." });
            }
        }

        /// <summary>
        /// Actualiza un rol.
        /// </summary>
        /// <param name="id">ID del rol.</param>
        /// <param name="roleDto">Datos del rol a actualizar.</param>
        /// <returns>Resultado de la actualización.</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleCreateDto roleDto)
        {
            _logger.LogInformation("Updating role with ID: {Id}", id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for role update with ID: {Id}", id);
                return BadRequest(ModelState);
            }

            try
            {
                var role = await _context.Role.FindAsync(id);
                if (role == null)
                {
                    _logger.LogWarning("Role with ID {Id} not found for update", id);
                    return NotFound(new { message = "Role not found" });
                }

                _mapper.Map(roleDto, role);
                _context.Entry(role).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Role with ID {Id} updated successfully", id);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(id))
                {
                    _logger.LogWarning("Role with ID {Id} not found during concurrency check", id);
                    return NotFound(new { message = "Role not found" });
                }
                else
                {
                    _logger.LogError("Concurrency error updating role with ID {Id}", id);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role with ID {Id}", id);
                return StatusCode(500, new { message = "Internal server error. Please try again later." });
            }
        }

        /// <summary>
        /// Crea un nuevo rol.
        /// </summary>
        /// <param name="roleDto">Datos del nuevo rol.</param>
        /// <returns>Rol creado con ID.</returns>
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateRole([FromBody] RoleCreateDto roleDto)
        {
            _logger.LogInformation("Creating a new role");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for role creation");
                return BadRequest(ModelState);
            }

            try
            {
                if (string.IsNullOrEmpty(roleDto.Description_Role))
                {
                    _logger.LogWarning("Description_Role is required for role creation");
                    return BadRequest("Description_Role is required.");
                }

                var role = _mapper.Map<Role>(roleDto);
                _context.Role.Add(role);
                await _context.SaveChangesAsync();

                var createdRoleDto = _mapper.Map<RoleDto>(role);

                _logger.LogInformation("Role created successfully with ID: {Id}", createdRoleDto.IdRole);
                return CreatedAtAction(nameof(GetRole), new { id = createdRoleDto.IdRole }, createdRoleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role");
                return StatusCode(500, new { message = "Internal server error. Please try again later." });
            }
        }

        /// <summary>
        /// Elimina un rol por ID.
        /// </summary>
        /// <param name="id">ID del rol a eliminar.</param>
        /// <returns>NoContent si se elimina correctamente.</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            _logger.LogInformation("Deleting role with ID: {Id}", id);

            try
            {
                var role = await _context.Role.FindAsync(id);
                if (role == null)
                {
                    _logger.LogWarning("Role with ID {Id} not found for deletion", id);
                    return NotFound(new { message = "Role not found" });
                }

                _context.Role.Remove(role);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Role with ID {Id} deleted successfully", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting role with ID {Id}", id);
                return StatusCode(500, new { message = "Internal server error. Please try again later." });
            }
        }

        private bool RoleExists(int id)
        {
            try
            {
                _logger.LogInformation("Checking if role with ID {Id} exists", id);
                var exists = _context.Role.Any(e => e.IdRole == id);
                _logger.LogInformation("Role with ID {Id} exists: {Exists}", id, exists);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if role with ID {Id} exists", id);
                return false;
            }
        }
    }
}
