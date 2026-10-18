namespace Backend.Models;

public class Patient
{
    public int PatientId { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime? BirthDate { get; set; }

    public string? Gender { get; set; }

    public bool IsRegistered { get; set; }

    public string? PasswordHash { get; set; }

    //Registered users data
    public string? SocialSecurityNumber { get; set; }
    public string? TaxNumber { get; set; }

    public string? Religion { get; set; }

    public string? DriversLicenseNumber { get; set; }

    public string? MedicalInsuranceNumber { get; set; }

    public ICollection<Appointment> Appointments { get; set; } =
    new List<Appointment>();


}