using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;
using Backend.DTOs;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services
{
    public class AuthService
    {
        private readonly AppointmentBookingContext _dataContext;
        private readonly JwtSettings _jwtSettings;
        private readonly PasswordHasher<Patient> _passwordHasher = new();

        public AuthService(
            AppointmentBookingContext dataContext, IOptions<JwtSettings> jwtSettings)
        {
            _dataContext = dataContext;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<bool> IsEmailAlreadyRegisteredAsync(
            string email,
            int? excludePatientId = null)
        {
            var normalizedEmail = email.Trim().ToLower();

            var patients = _dataContext.Patients
            .Where(p => p.IsRegistered);

            if (excludePatientId.HasValue)
            {
                patients = patients.Where(p => p.PatientId != excludePatientId.Value);
            }

            return await patients.AnyAsync(
                p => p.Email.ToLower() == normalizedEmail);
        }

        public async Task<PatientResponseDto?> RegisterAsync(RegisterPatientDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
               string.IsNullOrWhiteSpace(dto.Password) ||
               string.IsNullOrWhiteSpace(dto.FirstName) ||
               string.IsNullOrWhiteSpace(dto.LastName))
            {
                return null;
            }

            if (await IsEmailAlreadyRegisteredAsync(dto.Email))
            {
                return null;
            }

            var patient = new Patient
            {
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Email = dto.Email.Trim().ToLower(),
                BirthDate = dto.BirthDate,
                Gender = dto.Gender?.Trim(),
                IsRegistered = true,
                SocialSecurityNumber = dto.SocialSecurityNumber?.Trim(),
                TaxNumber = dto.TaxNumber?.Trim(),
                Religion = dto.Religion?.Trim(),
                DriversLicenseNumber = dto.DriversLicenseNumber?.Trim(),
                MedicalInsuranceNumber = dto.MedicalInsuranceNumber?.Trim()
            };

            patient.PasswordHash = _passwordHasher.HashPassword(
                patient,
                dto.Password);

            _dataContext.Patients.Add(patient);
            await _dataContext.SaveChangesAsync();

            return ToResponseDto(patient);

        }

        public async Task<PatientResponseDto?> UpdatePatientAsync(
            int patientId,
            UpdatePatientDto dto)
        {
            var patient = await _dataContext.Patients
            .FirstOrDefaultAsync(p => p.PatientId == patientId && p.IsRegistered);

            if (patient == null)
            {
                return null;
            }

            patient.FirstName = dto.FirstName.Trim();
            patient.LastName = dto.LastName.Trim();
            patient.Email = dto.Email.Trim().ToLower();
            patient.BirthDate = dto.BirthDate;
            patient.Gender = dto.Gender?.Trim();

            if (!string.IsNullOrWhiteSpace(dto.DriversLicenseNumber))
            {
                patient.DriversLicenseNumber = dto.DriversLicenseNumber.Trim();
            }


            if (!string.IsNullOrWhiteSpace(dto.MedicalInsuranceNumber))
            {
                patient.MedicalInsuranceNumber = dto.MedicalInsuranceNumber.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.Religion))
            {
                patient.Religion = dto.Religion.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.TaxNumber))
            {
                patient.TaxNumber = dto.TaxNumber.Trim();
            }

            if (!string.IsNullOrWhiteSpace(dto.SocialSecurityNumber))
            {
                patient.SocialSecurityNumber = dto.SocialSecurityNumber.Trim();
            }


            await _dataContext.SaveChangesAsync();

            return ToResponseDto(patient);
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) ||
               string.IsNullOrWhiteSpace(dto.Password))
            {
                return null;
            }

            var normalizedEmail = dto.Email.Trim().ToLower();

            var patient = await _dataContext.Patients.FirstOrDefaultAsync(p =>
            p.IsRegistered && p.Email.ToLower() == normalizedEmail);

            if (patient?.PasswordHash == null)
            {
                return null;
            }

            var passwordResult = _passwordHasher.VerifyHashedPassword(
                patient,
                patient.PasswordHash,
                dto.Password);

            if (passwordResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            var expiresAtUtc = DateTime.UtcNow.AddMinutes(
                _jwtSettings.ExpiryMinutes);

            return new LoginResponseDto
            {
                Token = GenerateToken(patient, expiresAtUtc),
                ExpiresAtUtc = expiresAtUtc,
                Patient = ToResponseDto(patient)
            };
        }

        private string GenerateToken(Patient patient, DateTime expiresAtUtc)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, patient.PatientId.ToString()),
                new Claim(ClaimTypes.Email, patient.Email)
            };

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Secret)
            );

            var signingCredentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiresAtUtc,
                signingCredentials: signingCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static PatientResponseDto ToResponseDto(Patient patient)
        {
            return new PatientResponseDto
            {
                PatientId = patient.PatientId,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Email = patient.Email,
                BirthDate = patient.BirthDate,
                Gender = patient.Gender,
                IsRegistered = patient.IsRegistered
            };
        }

    }
}