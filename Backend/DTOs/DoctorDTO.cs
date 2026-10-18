namespace Backend.DTOs;

public class CreateDoctorDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int SpecialityId { get; set; }
    public int ClinicId { get; set; }
}

public class DoctorResponseDto
{
    public int DoctorId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int SpecialityId { get; set; }
    public string SpecialityName { get; set; } = string.Empty;
    public int ClinicId { get; set; }
    public string ClinicName { get; set; } = string.Empty;
}

public class DoctorSearchResponseDto
{
    public string DoctorFullName { get; set; } = string.Empty;
    public string ClinicName { get; set; } = string.Empty;
    public string SpecialityName { get; set; } = string.Empty;
}