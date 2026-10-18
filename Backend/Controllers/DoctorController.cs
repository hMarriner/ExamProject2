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
    [Authorize]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly AppointmentBookingContext _context;

        public DoctorController(AppointmentBookingContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all doctors.
        /// </summary>

        //GET all Doctors
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DoctorResponseDto>>> GetDoctors()
        {
            var doctors = await _context.Doctors
                .Select(d => new DoctorResponseDto
                {
                    DoctorId = d.DoctorId,
                    FirstName = d.FirstName,
                    LastName = d.LastName,
                    SpecialityId = d.SpecialityId,
                    SpecialityName = d.Speciality != null ? d.Speciality.Name : string.Empty,
                    ClinicId = d.ClinicId,
                    ClinicName = d.Clinic != null ? d.Clinic.Name : string.Empty
                })
                .ToListAsync();

            return Ok(doctors);
        }

        /// <summary>
        /// Gets a doctor by its ID.
        /// </summary>

        //GET Doctor by Id
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DoctorResponseDto>> GetDoctor(int id)
        {
            var doctor = await _context.Doctors
                 .Where(d => d.DoctorId == id)
                 .Select(d => new DoctorResponseDto
                 {
                     DoctorId = d.DoctorId,
                     FirstName = d.FirstName,
                     LastName = d.LastName,
                     SpecialityId = d.SpecialityId,
                     SpecialityName = d.Speciality != null ? d.Speciality.Name : string.Empty,
                     ClinicId = d.ClinicId,
                     ClinicName = d.Clinic != null ? d.Clinic.Name : string.Empty
                 })
                 .FirstOrDefaultAsync();

            if (doctor == null)
            {
                return NotFound();
            }

            return Ok(doctor);
        }

        /// <summary>
        /// Searches for doctors based on a query string.
        /// </summary>

        //GET Doctor by search criteria 
        [HttpGet("search")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DoctorSearchResponseDto>>> SearchDoctors([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query cannot be empty.");
            }

            var searchTerm = query.Trim();

            var result = await _context.Doctors
                .Where(d => d.FirstName.ToLower().Contains(searchTerm.ToLower()) ||
                            d.LastName.ToLower().Contains(searchTerm.ToLower()))
                .Select(d => new DoctorSearchResponseDto
                {
                    DoctorFullName = $"{d.FirstName.Trim()} {d.LastName.Trim()}",
                    ClinicName = d.Clinic != null ? d.Clinic.Name : string.Empty,
                    SpecialityName = d.Speciality != null ? d.Speciality.Name : string.Empty
                })
                .Distinct()
                .ToListAsync();

            if (result.Count == 0)
            {
                return NotFound("No doctors found matching the search criteria.");
            }

            return Ok(result);
        }

        /// <summary>
        /// Creates a new doctor.
        /// </summary>

        //POST a new Doctor
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<DoctorResponseDto>> CreateDoctor(CreateDoctorDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
            {
                return BadRequest("First name and last name cannot be empty.");
            }

            var firstName = dto.FirstName.Trim();
            var lastName = dto.LastName.Trim();

            var speciality = await _context.Specialities.FindAsync(dto.SpecialityId);
            var clinic = await _context.Clinics.FindAsync(dto.ClinicId);

            if (speciality == null || clinic == null)
            {
                return BadRequest("Invalid SpecialityId or ClinicId.");
            }

            var duplicateExists = await _context.Doctors
            .AnyAsync(d =>
            d.FirstName.ToLower() == firstName.ToLower() &&
            d.LastName.ToLower() == lastName.ToLower() &&
            d.SpecialityId == dto.SpecialityId &&
            d.ClinicId == dto.ClinicId);

            if (duplicateExists)
            {
                return Conflict("A doctor with the same first name, last name, speciality, and clinic already exists.");
            }

            var doctor = new Doctor
            {
                FirstName = firstName,
                LastName = lastName,
                SpecialityId = dto.SpecialityId,
                ClinicId = dto.ClinicId
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            var responseDto = new DoctorResponseDto
            {
                DoctorId = doctor.DoctorId,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                SpecialityId = doctor.SpecialityId,
                SpecialityName = speciality.Name,
                ClinicId = doctor.ClinicId,
                ClinicName = clinic.Name
            };

            return CreatedAtAction(
                nameof(GetDoctor),
                new { id = doctor.DoctorId },
                responseDto);

        }

        /// <summary>
        /// Updates an existing doctor.
        /// </summary>

        //PUT update an existing Doctor
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]

        public async Task<ActionResult<DoctorResponseDto>> UpdateDoctor(int id, CreateDoctorDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
            {
                return BadRequest("First name and last name cannot be empty.");
            }

            var firstName = dto.FirstName.Trim();
            var lastName = dto.LastName.Trim();

            var existingDoctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.DoctorId == id);

            if (existingDoctor == null)
            {
                return NotFound("Doctor not found.");
            }
            var speciality = await _context.Specialities.FindAsync(dto.SpecialityId);
            var clinic = await _context.Clinics.FindAsync(dto.ClinicId);

            if (speciality == null || clinic == null)
            {
                return BadRequest("Invalid SpecialityId or ClinicId.");
            }

            var duplicateExists = await _context.Doctors
           .AnyAsync(d =>
           d.DoctorId != id &&
           d.FirstName.ToLower() == firstName.ToLower() &&
           d.LastName.ToLower() == lastName.ToLower() &&
           d.SpecialityId == dto.SpecialityId &&
           d.ClinicId == dto.ClinicId);

            if (duplicateExists)
            {
                return Conflict("A doctor with the same first name, last name, speciality, and clinic already exists.");
            }

            existingDoctor.FirstName = firstName;
            existingDoctor.LastName = lastName;
            existingDoctor.SpecialityId = dto.SpecialityId;
            existingDoctor.ClinicId = dto.ClinicId;

            await _context.SaveChangesAsync();

            var responseDto = new DoctorResponseDto
            {
                DoctorId = existingDoctor.DoctorId,
                FirstName = existingDoctor.FirstName,
                LastName = existingDoctor.LastName,
                SpecialityId = existingDoctor.SpecialityId,
                SpecialityName = speciality.Name,
                ClinicId = existingDoctor.ClinicId,
                ClinicName = clinic.Name
            };

            return Ok(responseDto);
        }

        /// <summary>
        /// Deletes an existing doctor by its ID.
        /// </summary>

        //DELETE an existing Doctor
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var existingDoctor = await _context.Doctors
                .FindAsync(id);

            if (existingDoctor == null)
            {
                return NotFound("Doctor not found.");
            }

            var hasAppointments = await _context.Appointments
            .AnyAsync(a => a.DoctorId == id);

            if (hasAppointments)
            {
                return Conflict("Cannot delete doctor with existing appointments.");
            }



            _context.Doctors.Remove(existingDoctor);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}