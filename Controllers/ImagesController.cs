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
    public class ImagesController : ControllerBase
    {
        private readonly ChiropracticContext _context;
        private readonly IMapper _mapper;

        public ImagesController(ChiropracticContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Images
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ImageDto>>> GetImages()
        {
            var images = await _context.Image.ToListAsync();
            var imageDtos = _mapper.Map<IEnumerable<ImageDto>>(images);
            return Ok(imageDtos);
        }

        // GET: api/Images/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ImageDto>> GetImage(int id)
        {
            var image = await _context.Image.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }

            var imageDto = _mapper.Map<ImageDto>(image);
            return Ok(imageDto);
        }

        // PUT: api/Images
        [HttpPut]
        public async Task<IActionResult> PutImage(int id, ImageCreateDto imageDto)
        {
            var image = await _context.Image.FindAsync(id);

            if (image == null)
            {
                return NotFound();
            }

            _mapper.Map(imageDto, image);
            _context.Entry(image).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImageExists(id))
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

        // POST: api/Images
        [HttpPost]
        public async Task<ActionResult<ImageDto>> PostImage(ImageCreateDto imageDto)
        {
            var image = _mapper.Map<Image>(imageDto);

            _context.Image.Add(image);
            await _context.SaveChangesAsync();

            var createdImageDto = _mapper.Map<ImageDto>(image);

            return CreatedAtAction(nameof(GetImage), new { id = createdImageDto.IdImage }, createdImageDto);
        }

        // DELETE: api/Images/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _context.Image.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }

            _context.Image.Remove(image);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ImageExists(int id)
        {
            return _context.Image.Any(e => e.IdImage == id);
        }
    }
}
