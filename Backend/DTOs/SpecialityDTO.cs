namespace Backend.DTOs;

public class SpecialityDto
{
    public string Name { get; set; } = string.Empty;
}

public class SpecialityResponseDto
{
    public int SpecialityId { get; set; }
    public string Name { get; set; } = string.Empty;

}