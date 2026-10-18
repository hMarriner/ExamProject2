namespace Backend.Models;

public class Appointment
{
    public int AppointmentId { get; set; }

    public int ClinicId { get; set; }

    public int DoctorId { get; set; }

    public DateTime Date { get; set; }

    public int DurationInMinutes { get; set; }

    public int PatientId { get; set; }

    public int AppointmentCategoryId { get; set; }

    public Clinic? Clinic { get; set; }
    public Doctor? Doctor { get; set; }
    public Patient? Patient { get; set; }
    public AppointmentCategory? AppointmentCategory { get; set; }
}