using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]

    public class PatientController : ControllerBase
    {
        private readonly AppointmentBookingContext _context;
        private readonly AuthService _authService;

        public PatientController(AppointmentBookingContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        /// <summary>
        /// Gets the profile of the currently logged-in patient.
        /// </summary>
        /// <returns></returns>

        //GET Logged in patient details
        [HttpGet("me")]

        [ProducesResponseType(typeof(PatientResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<PatientResponseDto>> GetMyProfile()
        {
            var patientIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(patientIdValue, out var patientId))
            {
                return Unauthorized();
            }

            var patient = await _context.Patients
            .Where(p => p.PatientId == patientId)
            .Select(p => new PatientResponseDto
            {
                PatientId = p.PatientId,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                BirthDate = p.BirthDate,
                Gender = p.Gender,
                IsRegistered = p.IsRegistered
            })
            .FirstOrDefaultAsync();

            if (patient == null)
            {
                return NotFound("Patient not found.");
            }

            return Ok(patient);
        }

        /// <summary>
        /// Updates the profile of the currently logged-in patient.
        /// </summary>

        //PUT update the profile of the currently logged-in patient.
        [HttpPut("me")]
        [ProducesResponseType(typeof(PatientResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<PatientResponseDto>> UpdateMyProfile(UpdatePatientDto updateDto)
        {

            var patientIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(patientIdValue, out var patientId))
            {
                return Unauthorized();
            }

            var patient = await _context.Patients.FindAsync(patientId);

            if (patient == null)
            {
                return NotFound("Patient not found.");
            }
            if (await _authService.IsEmailAlreadyRegisteredAsync(updateDto.Email, patientId))
            {
                return Conflict("Email is already registered.");
            }

            patient.FirstName = updateDto.FirstName;
            patient.LastName = updateDto.LastName;
            patient.Email = updateDto.Email;
            patient.BirthDate = updateDto.BirthDate;
            patient.Gender = updateDto.Gender;
            patient.MedicalInsuranceNumber = updateDto.MedicalInsuranceNumber;
            patient.Religion = updateDto.Religion;
            patient.DriversLicenseNumber = updateDto.DriversLicenseNumber;
            patient.TaxNumber = updateDto.TaxNumber;
            patient.SocialSecurityNumber = updateDto.SocialSecurityNumber;

            _context.Patients.Update(patient);
            await _context.SaveChangesAsync();

            var responseDto = new PatientResponseDto
            {
                PatientId = patient.PatientId,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Email = patient.Email,
                BirthDate = patient.BirthDate,
                Gender = patient.Gender,
                IsRegistered = patient.IsRegistered
            };

            return Ok(responseDto);
        }

        /// <summary>
        /// Deletes the profile of the currently logged-in patient.
        /// </summary>

        //DELETE Patient
        [HttpDelete("me")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeleteMyProfile()
        {
            var patientIdValue = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!int.TryParse(patientIdValue, out var patientId))
            {
                return Unauthorized();
            }

            var patient = await _context.Patients.FindAsync(patientId);

            if (patient == null)
            {
                return NotFound("Patient not found.");
            }
            var hasAppointments = await _context.Appointments
           .AnyAsync(a => a.PatientId == patientId);

            if (hasAppointments)
            {
                return Conflict("Cannot delete patient with existing appointments.");
            }

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}