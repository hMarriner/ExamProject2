namespace Backend.DTOs;

public class CreateAppointmentDto
{
    public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }

    public int DurationInMinutes { get; set; }
    public int AppointmentCategoryId { get; set; }
}

public class AppointmentResponseDto
{
    public int AppointmentId { get; set; }
    public int ClinicId { get; set; }
    public string ClinicName { get; set; } = string.Empty;
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }

    public int DurationInMinutes { get; set; }
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int AppointmentCategoryId { get; set; }
    public string AppointmentCategoryName { get; set; } = string.Empty;
}

public class GuestAppointmentDto
{
    public int DoctorId { get; set; }
    public DateTime AppointmentDate { get; set; }

    public int DurationInMinutes { get; set; }
    public int AppointmentCategoryId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public DateTime BirthDate { get; set; }

    public string? Gender { get; set; }


}