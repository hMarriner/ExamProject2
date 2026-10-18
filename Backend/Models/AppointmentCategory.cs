namespace Backend.Models;

public class AppointmentCategory
{
    public int AppointmentCategoryId { get; set; }
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public ICollection<Appointment> Appointments { get; set; } =
    new List<Appointment>();
}