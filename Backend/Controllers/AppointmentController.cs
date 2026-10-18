using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Backend.DTOs;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Backend.Services;

namespace Backend.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentBookingContext _context;
        private readonly AppointmentService _appointmentService;

        public AppointmentController(AppointmentBookingContext context, AppointmentService appointmentService)
        {
            _context = context;
            _appointmentService = appointmentService;
        }

        private bool TryGetCurrentPatientId(out int patientId)
        {
            patientId = 0;

            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);


            return !string.IsNullOrWhiteSpace(claim) && int.TryParse(claim, out patientId);

        }

        private IQueryable<Appointment> AppointmentDetails()
        {
            return _context.Appointments
            .AsNoTracking()
            .Include(a => a.Clinic)
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .Include(a => a.AppointmentCategory);
        }

        private static AppointmentResponseDto ToResponseDto(Appointment a)
        {
            return new AppointmentResponseDto
            {
                AppointmentId = a.AppointmentId,
                ClinicId = a.ClinicId,
                ClinicName = a.Clinic != null ? a.Clinic.Name : string.Empty,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor != null ? $"{a.Doctor.FirstName} {a.Doctor.LastName}".Trim() : string.Empty,
                PatientId = a.PatientId,
                PatientName = a.Patient != null ? $"{a.Patient.FirstName} {a.Patient.LastName}".Trim() : string.Empty,
                AppointmentCategoryId = a.AppointmentCategoryId,
                AppointmentCategoryName = a.AppointmentCategory != null ? a.AppointmentCategory.Name : string.Empty,
                AppointmentDate = a.Date,
                DurationInMinutes = a.DurationInMinutes
            };

        }

        private static string? ValidateDuration(int durationInMinutes)
        {
            if (durationInMinutes < AppointmentService.MinimumAppointmentDurationInMinutes ||
                durationInMinutes > AppointmentService.MaximumAppointmentDurationInMinutes)
            {
                return $"Duration must be between 10 and 30 minutes.";
            }

            return null;
        }

        private static string? ValidateAppointmentTime(
            DateTime appointmentDate,
            int durationInMinutes)
        {
            var durationError = ValidateDuration(durationInMinutes);
            if (durationError != null)
            {
                return durationError;
            }

            if (!AppointmentService.IsAppointmentTimeValid(
                appointmentDate,
                durationInMinutes))
            {
                return "Choose a future appointment time between 9 AM and 5 PM.";
            }

            return null;
        }
        private static string? ValidateGuestDetails(GuestAppointmentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FirstName) ||
              string.IsNullOrWhiteSpace(dto.LastName) ||
              string.IsNullOrWhiteSpace(dto.Email) ||
              dto.BirthDate == default)
            {
                return "First name, last name, email, and birth date are required.";
            }

            if (dto.BirthDate > DateTime.Today)
            {
                return "Birth date cannot be in the future.";
            }

            return null;

        }

        private async Task<int?> GetClinicIdForAppointmentAsync(
            int doctorId,
            int appointmentCategoryId)
        {
            var doctor = await _context.Doctors
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.DoctorId == doctorId);

            if (doctor == null)
            {
                return null;
            }

            var categoryExists = await _context.AppointmentCategories
            .AnyAsync(ac => ac.AppointmentCategoryId == appointmentCategoryId);

            if (!categoryExists)
            {
                return null;
            }

            var clinicExists = await _context.Clinics
            .AnyAsync(c => c.ClinicId == doctor.ClinicId);

            return clinicExists ? doctor.ClinicId : null;
        }

        /// <summary>
        /// Gets all appointments for the current authenticated patient.
        /// </summary>

        //GET all Appointments for the current authenticated patient
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<AppointmentResponseDto>>> GetAppointments()
        {
            if (!TryGetCurrentPatientId(out var patientId))
            {
                return Unauthorized();
            }

            var appointments = await AppointmentDetails()
                .Where(a => a.PatientId == patientId)
                .OrderBy(a => a.Date)
                .ToListAsync();

            return Ok(appointments.Select(ToResponseDto));
        }


        /// <summary>
        /// Gets an appointment by its ID for the current authenticated patient.
        /// </summary>

        //GET Appointment by {Id} for the current authenticated patient
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AppointmentResponseDto>> GetAppointment(int id)
        {
            if (!TryGetCurrentPatientId(out var patientId))
            {
                return Unauthorized();
            }

            var appointment = await AppointmentDetails()
                .FirstOrDefaultAsync(a => a.AppointmentId == id && a.PatientId == patientId);

            if (appointment == null)
            {
                return NotFound("Appointment not found or not owned by this patient.");
            }
            return Ok(ToResponseDto(appointment));
        }

        /// <summary>
        /// Gets the available appointment slots for a specific doctor on a given date.
        /// </summary>

        //GET available appointment slots for a specific doctor on a given date
        [HttpGet("availability")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DateTime>>> GetAvailability(
            [FromQuery] int doctorId,
            [FromQuery] DateTime date,
            [FromQuery] int durationInMinutes = 10)
        {
            var durationError = ValidateDuration(durationInMinutes);
            if (durationError != null)
            {
                return BadRequest(durationError);
            }

            if (date == default || date.Date < DateTime.Today)
            {
                return BadRequest("Invalid date. Date must be today or later.");
            }

            var doctorExists = await _context.Doctors
            .AnyAsync(d => d.DoctorId == doctorId);

            if (!doctorExists)
            {
                return NotFound("Doctor not found.");
            }

            var availableSlots = await _appointmentService.GetAvailableSlotsAsync(
                doctorId,
                date.Date,
                durationInMinutes);

            return Ok(availableSlots);
        }

        /// <summary>
        /// Creates a new appointment for the current authenticated patient.
        /// </summary>

        //POST a new Appointment for the current authenticated patient
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AppointmentResponseDto>> CreateAppointment([FromBody] CreateAppointmentDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Appointment request is required.");
            }
            if (!TryGetCurrentPatientId(out var patientId))
            {
                return Unauthorized();
            }

            var timeError = ValidateAppointmentTime(
                dto.AppointmentDate,
                dto.DurationInMinutes);

            if (timeError != null)
            {
                return BadRequest(timeError);
            }

            var clinicId = await GetClinicIdForAppointmentAsync(
                dto.DoctorId,
                dto.AppointmentCategoryId);

            if (clinicId == null)
            {
                return NotFound("Invalid doctor or appoinment category.");
            }


            if (await _appointmentService.HasConflictAsync(
                patientId,
                dto.DoctorId,
                dto.AppointmentDate,
                dto.DurationInMinutes))
            {
                return Conflict("The patient or doctor is already booked at this time.");
            }

            var appointment = new Appointment
            {
                ClinicId = clinicId.Value,
                DoctorId = dto.DoctorId,
                PatientId = patientId,
                AppointmentCategoryId = dto.AppointmentCategoryId,
                Date = dto.AppointmentDate,
                DurationInMinutes = dto.DurationInMinutes,


            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            var createdAppointment = await AppointmentDetails()
            .FirstAsync(a => a.AppointmentId == appointment.AppointmentId);


            return CreatedAtAction(
                nameof(GetAppointment),
                new { id = appointment.AppointmentId },
                ToResponseDto(createdAppointment));
        }

        /// <summary>
        /// Creates a new guest appointment.
        /// </summary>

        //POST guest appointment
        [HttpPost("guest-booking")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AppointmentResponseDto>> GuestBooking([FromBody] GuestAppointmentDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Appointment request is required.");
            }

            var timeError = ValidateAppointmentTime(
                dto.AppointmentDate,
                dto.DurationInMinutes);

            if (timeError != null)
            {
                return BadRequest(timeError);
            }

            var patientError = ValidateGuestDetails(dto);
            if (patientError != null)
            {
                return BadRequest(patientError);
            }

            var clinicId = await GetClinicIdForAppointmentAsync(
                dto.DoctorId,
                dto.AppointmentCategoryId);

            if (clinicId == null)
            {
                return NotFound("Invalid doctor or appoinment category.");
            }

            if (await _appointmentService.HasConflictAsync(
                null,
                dto.DoctorId,
                dto.AppointmentDate,
                dto.DurationInMinutes))
            {
                return Conflict("The doctor is already booked at this time.");
            }


            var patient = new Patient
            {
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Email = dto.Email.Trim(),
                BirthDate = dto.BirthDate,
                Gender = dto.Gender?.Trim(),
                IsRegistered = false,
                PasswordHash = null
            };

            var appointment = new Appointment
            {
                ClinicId = clinicId.Value,
                DoctorId = dto.DoctorId,
                Patient = patient,
                AppointmentCategoryId = dto.AppointmentCategoryId,
                Date = dto.AppointmentDate,
                DurationInMinutes = dto.DurationInMinutes,
            };

            _context.Patients.Add(patient);
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            var createdAppointment = await AppointmentDetails()
            .FirstAsync(a => a.AppointmentId == appointment.AppointmentId);

            return StatusCode(StatusCodes.Status201Created,
            ToResponseDto(createdAppointment));
        }

        /// <summary>
        /// Updates an existing appointment for the current authenticated patient.
        /// </summary>

        //PUT update Appointment for the current authenticated patient
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AppointmentResponseDto>> UpdateAppointment(int id, [FromBody] CreateAppointmentDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Appointment request is required.");
            }

            if (!TryGetCurrentPatientId(out var patientId))
            {
                return Unauthorized();
            }

            var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.AppointmentId == id && a.PatientId == patientId);

            if (appointment == null)
            {
                return NotFound("Appointment not found or not owned by this patient.");
            }
            var timeError = ValidateAppointmentTime(
            dto.AppointmentDate,
            dto.DurationInMinutes);

            if (timeError != null)
            {
                return BadRequest(timeError);
            }

            var clinicId = await GetClinicIdForAppointmentAsync(
                 dto.DoctorId,
                 dto.AppointmentCategoryId);

            if (clinicId == null)
            {
                return NotFound("Invalid doctor or appointment category.");
            }

            if (await _appointmentService.HasConflictAsync(
                patientId,
                dto.DoctorId,
                dto.AppointmentDate,
                dto.DurationInMinutes,
                id))
            {
                return Conflict("The patient or doctor is already booked at this time.");
            }

            appointment.ClinicId = clinicId.Value;
            appointment.DoctorId = dto.DoctorId;
            appointment.AppointmentCategoryId = dto.AppointmentCategoryId;
            appointment.Date = dto.AppointmentDate;
            appointment.DurationInMinutes = dto.DurationInMinutes;

            await _context.SaveChangesAsync();

            var updatedAppointment = await AppointmentDetails()
            .FirstAsync(a => a.AppointmentId == id);

            return Ok(ToResponseDto(updatedAppointment));
        }

        /// <summary>
        /// Deletes an existing appointment for the current authenticated patient.
        /// </summary>

        //DELETE an Appointment for the current authenticated patient
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> DeleteAppointment(int id)
        {

            if (!TryGetCurrentPatientId(out var patientId))
            {
                return Unauthorized();
            }
            var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.AppointmentId == id && a.PatientId == patientId);

            if (appointment == null)
            {
                return NotFound("Appointment not found");
            }
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}




