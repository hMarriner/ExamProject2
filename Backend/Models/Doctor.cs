namespace Backend.Models;

public class Doctor
{
    public int DoctorId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public int SpecialityId { get; set; }

    public int ClinicId { get; set; }

    public Speciality? Speciality { get; set; }

    public Clinic? Clinic { get; set; }

    public ICollection<Appointment> Appointments { get; set; } =
    new List<Appointment>();

}