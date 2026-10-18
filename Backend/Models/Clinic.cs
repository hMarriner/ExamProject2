namespace Backend.Models;

public class Clinic
{
    public int ClinicId { get; set; }
    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public ICollection<Doctor> Doctors { get; set; } =
    new List<Doctor>();

    public ICollection<Appointment> Appointments { get; set; } =
    new List<Appointment>();

}