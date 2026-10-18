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
    public class ClinicController : ControllerBase
    {
        private readonly AppointmentBookingContext _context;

        public ClinicController(AppointmentBookingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all clinics.
        /// </summary>

        //GET all Clinics
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ClinicResponseDto>>> GetClinics()
        {
            var clinics = await _context.Clinics
            .Select(c => new ClinicResponseDto
            {
                ClinicId = c.ClinicId,
                Name = c.Name,
                Address = c.Address
            })
            .ToListAsync();

            return Ok(clinics);
        }

        /// <summary>
        /// Gets a clinic by its ID.
        /// </summary>
        //GET Clinic by {Id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClinicResponseDto>> GetClinic(int id)
        {
            var clinic = await _context.Clinics
            .Where(c => c.ClinicId == id)
            .Select(c => new ClinicResponseDto
            {
                ClinicId = c.ClinicId,
                Name = c.Name,
                Address = c.Address
            }).FirstOrDefaultAsync();

            if (clinic == null)
            {
                return NotFound();
            }

            return Ok(clinic);
        }

        /// <summary>
        /// Creates a new clinic.
        /// </summary>

        //POST a new clinic
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ClinicResponseDto>> CreateClinic(ClinicDto dto)
        {

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Clinic name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(dto.Address))
            {
                return BadRequest("Clinic address cannot be empty.");
            }

            var name = dto.Name.Trim();
            var address = dto.Address.Trim();

            var clinicAlreadyExists = await _context.Clinics
            .AnyAsync(existingClinic =>
            existingClinic.Name.ToLower() == name.ToLower() &&
            existingClinic.Address.ToLower() == address.ToLower());

            if (clinicAlreadyExists)
            {
                return Conflict("A clinic with the same name and address already exists.");
            }

            var clinic = new Clinic
            {
                Name = name,
                Address = address
            };

            _context.Clinics.Add(clinic);
            await _context.SaveChangesAsync();

            var responseDto = new ClinicResponseDto
            {
                ClinicId = clinic.ClinicId,
                Name = clinic.Name,
                Address = clinic.Address
            };

            return CreatedAtAction(
                nameof(GetClinic),
                new { id = clinic.ClinicId },
                responseDto);

        }

        /// <summary>
        /// Updates an existing clinic.
        /// </summary>

        //PUT update an existing clinic
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]

        public async Task<ActionResult<ClinicResponseDto>> UpdateClinic(int id, ClinicDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return BadRequest("Clinic name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(dto.Address))
            {
                return BadRequest("Clinic address cannot be empty.");
            }

            var name = dto.Name.Trim();
            var address = dto.Address.Trim();

            var existingClinic = await _context.Clinics
            .FirstOrDefaultAsync(c => c.ClinicId == id);

            if (existingClinic == null)
            {
                return NotFound("Clinic not found.");
            }

            var duplicateExists = await _context.Clinics
            .AnyAsync(c =>
                c.ClinicId != id &&
                c.Name.ToLower() == name.ToLower() &&
                c.Address.ToLower() == address.ToLower());

            if (duplicateExists)
            {
                return Conflict("A clinic with the same name and address already exists.");
            }

            existingClinic.Name = name;
            existingClinic.Address = address;

            await _context.SaveChangesAsync();

            var responseDto = new ClinicResponseDto
            {
                ClinicId = existingClinic.ClinicId,
                Name = existingClinic.Name,
                Address = existingClinic.Address
            };

            return Ok(responseDto);
        }

        /// <summary>
        /// Deletes an existing clinic by its ID.
        /// </summary>

        //DELETE an existing Clinic
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteClinic(int id)
        {
            var existingClinic = await _context.Clinics
                .FindAsync(id);
            if (existingClinic == null)
            {
                return NotFound("Clinic not found.");
            }

            var hasAppointments = await _context.Appointments
            .AnyAsync(a => a.ClinicId == id);

            var hasDoctors = await _context.Doctors
            .AnyAsync(d => d.ClinicId == id);

            if (hasAppointments || hasDoctors)
            {
                return Conflict("Cannot delete clinic with existing appointments or doctors.");
            }

            _context.Clinics.Remove(existingClinic);
            await _context.SaveChangesAsync();

            return NoContent();
        }




    }
}