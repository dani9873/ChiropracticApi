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
    public class ImagesController : ControllerBase
    {
        private readonly ChiropracticContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ImagesController> _logger;

        public ImagesController(ChiropracticContext context, IMapper mapper, ILogger<ImagesController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todas las imágenes.
        /// </summary>
        /// <returns>Lista de ImageDto.</returns>
        [HttpGet]
        public async Task<IActionResult> GetImages()
        {
            _logger.LogInformation("Getting all images");

            try
            {
                var images = await _context.Image.ToListAsync();
                _logger.LogInformation("Retrieved {Count} images", images.Count);

                var imageDtos = _mapper.Map<IEnumerable<ImageDto>>(images);
                return Ok(imageDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving images");
                return StatusCode(500, new { message = "Internal server error. Please try again later." });
            }
        }

        /// <summary>
        /// Obtiene una imagen por ID.
        /// </summary>
        /// <param name="id">ID de la imagen.</param>
        /// <returns>ImageDto.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetImage(int id)
        {
            _logger.LogInformation("Getting image with ID: {Id}", id);

            try
            {
                var image = await _context.Image.FindAsync(id);
                if (image == null)
                {
                    _logger.LogWarning("Image with ID {Id} not found", id);
                    return NotFound(new { message = "Image not found" });
                }

                var imageDto = _mapper.Map<ImageDto>(image);
                _logger.LogInformation("Image with ID {Id} retrieved successfully", id);
                return Ok(imageDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving image with ID {Id}", id);
                return StatusCode(500, new { message = "Internal server error. Please try again later." });
            }
        }

        /// <summary>
        /// Actualiza una imagen.
        /// </summary>
        /// <param name="id">ID de la imagen.</param>
        /// <param name="imageDto">Datos de la imagen a actualizar.</param>
        /// <returns>Resultado de la actualización.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutImage(int id, [FromBody] ImageCreateDto imageDto)
        {
            _logger.LogInformation("Updating image with ID: {Id}", id);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for image update with ID: {Id}", id);
                return BadRequest(ModelState);
            }

            try
            {
                var image = await _context.Image.FindAsync(id);
                if (image == null)
                {
                    _logger.LogWarning("Image with ID {Id} not found for update", id);
                    return NotFound(new { message = "Image not found" });
                }

                _mapper.Map(imageDto, image);
                _context.Entry(image).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Image with ID {Id} updated successfully", id);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImageExists(id))
                {
                    _logger.LogWarning("Image with ID {Id} not found during concurrency check", id);
                    return NotFound(new { message = "Image not found" });
                }
                else
                {
                    _logger.LogError("Concurrency error updating image with ID {Id}", id);
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating image with ID {Id}", id);
                return StatusCode(500, new { message = "Internal server error. Please try again later." });
            }
        }

        /// <summary>
        /// Crea una nueva imagen.
        /// </summary>
        /// <param name="imageDto">Datos de la nueva imagen.</param>
        /// <returns>Imagen creada con ID.</returns>
        [HttpPost]
        public async Task<IActionResult> PostImage([FromBody] ImageCreateDto imageDto)
        {
            _logger.LogInformation("Creating a new image");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for image creation");
                return BadRequest(ModelState);
            }

            try
            {
                var image = _mapper.Map<Image>(imageDto);
                _context.Image.Add(image);
                await _context.SaveChangesAsync();

                var createdImageDto = _mapper.Map<ImageDto>(image);

                _logger.LogInformation("Image created successfully with ID: {Id}", createdImageDto.IdImage);
                return CreatedAtAction(nameof(GetImage), new { id = createdImageDto.IdImage }, createdImageDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating image");
                return StatusCode(500, new { message = "Internal server error. Please try again later." });
            }
        }

        /// <summary>
        /// Elimina una imagen por ID.
        /// </summary>
        /// <param name="id">ID de la imagen a eliminar.</param>
        /// <returns>NoContent si se elimina correctamente.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            _logger.LogInformation("Deleting image with ID: {Id}", id);

            try
            {
                var image = await _context.Image.FindAsync(id);
                if (image == null)
                {
                    _logger.LogWarning("Image with ID {Id} not found for deletion", id);
                    return NotFound(new { message = "Image not found" });
                }

                _context.Image.Remove(image);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Image with ID {Id} deleted successfully", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting image with ID {Id}", id);
                return StatusCode(500, new { message = "Internal server error. Please try again later." });
            }
        }

        private bool ImageExists(int id)
        {
            try
            {
                _logger.LogInformation("Checking if image with ID {Id} exists", id);
                var exists = _context.Image.Any(e => e.IdImage == id);
                _logger.LogInformation("Image with ID {Id} exists: {Exists}", id, exists);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if image with ID {Id} exists", id);
                return false;
            }
        }
    }
}
