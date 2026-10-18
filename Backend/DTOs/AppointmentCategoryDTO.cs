namespace Backend.DTOs
{
    public class AppointmentCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    public class AppointmentCategoryResponseDto
    {
        public int AppointmentCategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

    }
}