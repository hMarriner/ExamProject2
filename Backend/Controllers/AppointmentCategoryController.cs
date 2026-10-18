using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Backend.DTOs;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentCategoryController : ControllerBase
    {
        private readonly AppointmentBookingContext _context;

        public AppointmentCategoryController(AppointmentBookingContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Gets all appointment categories.
        /// </summary>
        /// 
        //GET all Appointment Categories
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AppointmentCategoryResponseDto>>> GetAppointmentCategories()
        {
            var categories = await _context.AppointmentCategories
            .Select(ac => new AppointmentCategoryResponseDto
            {
                AppointmentCategoryId = ac.AppointmentCategoryId,
                Name = ac.Name,
                Description = ac.Description
            }).ToListAsync();

            return Ok(categories);
        }

        /// <summary>
        /// Gets an appointment category by its ID.
        /// </summary>

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AppointmentCategoryResponseDto>> GetAppointmentCategory(int id)
        {
            var appointmentCategory = await _context.AppointmentCategories
            .Where(ac => ac.AppointmentCategoryId == id)
            .Select(ac => new AppointmentCategoryResponseDto
            {
                AppointmentCategoryId = ac.AppointmentCategoryId,
                Name = ac.Name,
                Description = ac.Description
            })
            .FirstOrDefaultAsync();

            if (appointmentCategory == null)
            {
                return NotFound();
            }
            return appointmentCategory;
        }

        /// <summary>
        /// Creates a new appointment category.
        /// </summary>

        //POST a new Appointment Category
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AppointmentCategoryResponseDto>> CreateAppointmentCategory(AppointmentCategoryDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Appointment category name cannot be empty.");
            }

            var name = dto.Name.Trim();

            var nameExists = await _context.AppointmentCategories
            .AnyAsync(ac => ac.Name.ToLower() == name.ToLower());

            if (nameExists)
            {
                return Conflict("An appointment category with the same name already exists.");
            }

            var appointmentCategory = new AppointmentCategory
            {
                Name = name,
                Description = string.IsNullOrWhiteSpace(dto.Description)
                ? null
                : dto.Description.Trim()
            };


            _context.AppointmentCategories.Add(appointmentCategory);
            await _context.SaveChangesAsync();

            var responseDto = new AppointmentCategoryResponseDto
            {
                AppointmentCategoryId = appointmentCategory.AppointmentCategoryId,
                Name = appointmentCategory.Name,
                Description = appointmentCategory.Description
            };

            return CreatedAtAction(
                nameof(GetAppointmentCategory),
                new { id = appointmentCategory.AppointmentCategoryId },
                responseDto);

        }

        /// <summary>
        /// Update an appointment category by its ID.
        /// </summary>

        //PUT update an existing appointment category
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AppointmentCategoryResponseDto>> UpdateAppointmentCategory(int id, AppointmentCategoryDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Appointment category name cannot be empty.");
            }

            var name = dto.Name.Trim();

            var existingAppointmentCategory = await _context.AppointmentCategories
            .FirstOrDefaultAsync(ac => ac.AppointmentCategoryId == id);

            if (existingAppointmentCategory == null)
            {
                return NotFound("Appointment category not found.");
            }

            var duplicateExists = await _context.AppointmentCategories
            .AnyAsync(ac =>
                ac.AppointmentCategoryId != id &&
                ac.Name.ToLower() == name.ToLower());

            if (duplicateExists)
            {
                return Conflict("An appointment category with the same name already exists.");
            }

            existingAppointmentCategory.Name = name;
            existingAppointmentCategory.Description =
            string.IsNullOrWhiteSpace(dto.Description)
            ? null
            : dto.Description.Trim();

            await _context.SaveChangesAsync();

            var responseDto = new AppointmentCategoryResponseDto
            {
                AppointmentCategoryId = existingAppointmentCategory.AppointmentCategoryId,
                Name = existingAppointmentCategory.Name,
                Description = existingAppointmentCategory.Description
            };

            return Ok(responseDto);


        }

        /// <summary>
        /// Deletes an appointment category by its ID.
        /// </summary>

        //DELETE an Appointment category
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteAppointmentCategory(int id)
        {
            var existingAppointmentCategory = await _context.AppointmentCategories.FindAsync(id);
            if (existingAppointmentCategory == null)
            {
                return NotFound("Appointment category not found.");
            }

            var hasAppointments = await _context.Appointments
            .AnyAsync(a => a.AppointmentCategoryId == id);

            if (hasAppointments)
            {
                return Conflict("Cannot delete appointment category with existing appointments.");
            }
            _context.AppointmentCategories.Remove(existingAppointmentCategory);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
