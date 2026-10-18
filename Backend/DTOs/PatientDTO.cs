
using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    public class CreatePatientDto
    {

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTime? BirthDate { get; set; }

        public string? Gender { get; set; }

        public string? SocialSecurityNumber { get; set; }
        public string? TaxNumber { get; set; }

        public string? Religion { get; set; }

        public string? DriversLicenseNumber { get; set; }

        public string? MedicalInsuranceNumber { get; set; }
    }

    public class GuestUserDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTime? BirthDate { get; set; }

        public string? Gender { get; set; }
    }

    public class RegisterPatientDto
    {

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public DateTime? BirthDate { get; set; }

        public string? Gender { get; set; }


        public string? SocialSecurityNumber { get; set; }
        public string? TaxNumber { get; set; }

        public string? Religion { get; set; }

        public string? DriversLicenseNumber { get; set; }

        public string? MedicalInsuranceNumber { get; set; }
    }

    public class PatientResponseDto
    {
        public int PatientId { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public DateTime? BirthDate { get; set; }

        public string? Gender { get; set; }

        public bool IsRegistered { get; set; }

    }

    public class UpdatePatientDto
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        public DateTime? BirthDate { get; set; }
        [MaxLength(10)]
        public string? Gender { get; set; }

        [MaxLength(11)]
        public string? SocialSecurityNumber { get; set; }
        [MaxLength(50)]

        public string? TaxNumber { get; set; }
        [MaxLength(50)]
        public string? Religion { get; set; }

        [MaxLength(50)]
        public string? DriversLicenseNumber { get; set; }
        [MaxLength(50)]
        public string? MedicalInsuranceNumber { get; set; }
    }
}