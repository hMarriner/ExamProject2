using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Backend.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SpecialityController : ControllerBase
    {
        private readonly AppointmentBookingContext _context;

        public SpecialityController(AppointmentBookingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all specialities.
        /// </summary>

        //GET all Specialities
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SpecialityResponseDto>>> GetSpecialities()
        {
            var specialities = await _context.Specialities
            .Select(s => new SpecialityResponseDto
            {
                SpecialityId = s.SpecialityId,
                Name = s.Name
            }).ToListAsync();

            return Ok(specialities);
        }

        /// <summary>
        /// Gets a speciality by its ID.
        /// </summary>

        //GET Speciality by Id
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SpecialityResponseDto>> GetSpeciality(int id)
        {
            var speciality = await _context.Specialities
            .Where(s => s.SpecialityId == id)
            .Select(s => new SpecialityResponseDto
            {
                SpecialityId = s.SpecialityId,
                Name = s.Name
            })
            .FirstOrDefaultAsync();
            if (speciality == null)
            {
                return NotFound();
            }

            return Ok(speciality);
        }

        /// <summary>
        /// Creates a new speciality.
        /// </summary>

        //POST a new Speciality
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<SpecialityResponseDto>> CreateSpeciality(SpecialityDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Speciality name cannot be empty.");
            }
            var name = dto.Name.Trim();

            var nameExists = await _context.Specialities
            .AnyAsync(s => s.Name.ToLower() == name.ToLower());
            if (nameExists)
            {
                return Conflict("A speciality with the same name already exists.");
            }

            var speciality = new Speciality
            {
                Name = name
            };


            _context.Specialities.Add(speciality);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetSpeciality),
                new { id = speciality.SpecialityId },
                new SpecialityResponseDto
                {
                    SpecialityId = speciality.SpecialityId,
                    Name = speciality.Name
                });

        }

        /// <summary>
        /// Updates an existing speciality by its ID.
        /// </summary>

        //PUT update an existing Speciality
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]

        public async Task<ActionResult<SpecialityResponseDto>> UpdateSpeciality(int id, SpecialityDto dto)
        {

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Speciality name cannot be empty.");
            }

            var name = dto.Name.Trim();

            var existingSpeciality = await _context.Specialities
                .FindAsync(id);

            if (existingSpeciality == null)
            {
                return NotFound("Speciality not found.");
            }

            var duplicateExists = await _context.Specialities
             .AnyAsync(s => s.SpecialityId != id &&
             s.Name.ToLower() == name.ToLower());

            if (duplicateExists)
            {
                return Conflict("A speciality with the same name already exists.");
            }

            existingSpeciality.Name = name;

            await _context.SaveChangesAsync();

            var responseDto = new SpecialityResponseDto
            {
                SpecialityId = existingSpeciality.SpecialityId,
                Name = existingSpeciality.Name
            };

            return Ok(responseDto);
        }

        /// <summary>
        /// Deletes an existing speciality by its ID.
        /// </summary>

        //DELETE an existing Speciality
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteSpeciality(int id)
        {
            var existingSpeciality = await _context.Specialities
                .FindAsync(id);
            if (existingSpeciality == null)
            {
                return NotFound("Speciality not found.");
            }

            var hasDoctors = await _context.Doctors
            .AnyAsync(d => d.SpecialityId == id);

            if (hasDoctors)
            {
                return Conflict("Cannot delete speciality with existing doctors.");
            }

            _context.Specialities.Remove(existingSpeciality);
            await _context.SaveChangesAsync();

            return NoContent();
        }




    }
}