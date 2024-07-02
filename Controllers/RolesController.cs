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
    public class RoleController : ControllerBase
    {
        private readonly ChiropracticContext _context;
        private readonly IMapper _mapper;

        public RoleController(ChiropracticContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all roles.
        /// </summary>
        /// <returns>List of RoleDto</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetRoles()
        {
            var roles = await _context.Role.ToListAsync();
            var roleDtos = _mapper.Map<IEnumerable<RoleDto>>(roles);
            return Ok(roleDtos);
        }

        /// <summary>
        /// Gets a role by id.
        /// </summary>
        /// <param name="id">Role id</param>
        /// <returns>RoleDto</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetRole(int id)
        {
            var role = await _context.Role.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            var roleDto = _mapper.Map<RoleDto>(role);
            return Ok(roleDto);
        }

       // PUT: api/Roles
        [HttpPut]
        public async Task<IActionResult> UpdateRole(int id, RoleCreateDto roleDto)
        {
            var role = await _context.Role.FindAsync(id);

            if (role == null)
            {
                return NotFound();
            }

            _mapper.Map(roleDto, role);
            _context.Entry(role).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RoleExists(id))
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

        /// <summary>
        /// Creates a new role and returns the new role with the generated ID.
        /// </summary>
        /// <param name="roleDto">RoleDto</param>
        /// <returns>Created RoleDto with ID</returns>
        [HttpPost]
        public async Task<ActionResult<RoleDto>> CreateRole(RoleCreateDto roleDto)
        {
            if (string.IsNullOrEmpty(roleDto.Description_Role))
                {
                    return BadRequest("Description_Role is required.");
                }
            var role = _mapper.Map<Role>(roleDto);

            _context.Role.Add(role);
            await _context.SaveChangesAsync();

            var createdRoleDto = _mapper.Map<RoleDto>(role);

            return CreatedAtAction(nameof(GetRole), new { id = createdRoleDto.IdRole }, createdRoleDto);
        }

        /// <summary>
        /// Deletes a role by id.
        /// </summary>
        /// <param name="id">Role id</param>
        /// <returns>NoContent if successful</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Role.FindAsync(id);
            if (role == null)
            {
                return NotFound();
            }

            _context.Role.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RoleExists(int id)
        {
            return _context.Role.Any(e => e.IdRole == id);
        }
    }
}
