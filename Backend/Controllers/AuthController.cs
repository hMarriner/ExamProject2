using Microsoft.AspNetCore.Mvc;
using Backend.Services;
using Backend.DTOs;
using Microsoft.AspNetCore.Http;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Registers a new patient.
        /// </summary>

        //POST register a new patient
        [HttpPost("register")]
        [ProducesResponseType(typeof(PatientResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<PatientResponseDto>> Register([FromBody] RegisterPatientDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Request body is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Email) ||
            string.IsNullOrWhiteSpace(dto.Password) ||
            string.IsNullOrWhiteSpace(dto.FirstName) ||
            string.IsNullOrWhiteSpace(dto.LastName) ||
            dto.BirthDate == null ||
            string.IsNullOrWhiteSpace(dto.Gender) ||
            string.IsNullOrWhiteSpace(dto.SocialSecurityNumber) ||
            string.IsNullOrWhiteSpace(dto.Religion) ||
            string.IsNullOrWhiteSpace(dto.TaxNumber) ||
            string.IsNullOrWhiteSpace(dto.DriversLicenseNumber) ||
            string.IsNullOrWhiteSpace(dto.MedicalInsuranceNumber))

            {
                return BadRequest("Email, password, first name, last name, date of birth, gender, social security number, religion, tax number, drivers license number, and medical insurance number are required.");
            }

            if (dto.SocialSecurityNumber?.Length > 11)
            {
                return BadRequest("Social security number cannot exceed 11 characters.");
            }

            if (dto.TaxNumber?.Length > 50 ||
            dto.Religion?.Length > 50 ||
            dto.DriversLicenseNumber?.Length > 50 ||
            dto.MedicalInsuranceNumber?.Length > 50 ||
            dto.Gender?.Length > 50)
            {
                return BadRequest("Tax number, religion, drivers license number, medical insurance number, and gender cannot exceed 50 characters.");
            }

            if (await _authService.IsEmailAlreadyRegisteredAsync(dto.Email))
            {
                return Conflict("Email is already registered.");
            }

            var patient = await _authService.RegisterAsync(dto);

            if (patient == null)
            {
                return BadRequest("Registration failed.");
            }

            return StatusCode(StatusCodes.Status201Created, patient);
        }
        /// <summary>
        /// Authenticates a patient and returns a login response.
        /// </summary>

        //POST login a patient
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Request body is required.");
            }
            var loginResponse = await _authService.LoginAsync(dto);

            if (loginResponse == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            return Ok(loginResponse);
        }
    }
}