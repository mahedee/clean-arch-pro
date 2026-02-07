namespace EduTrack.Application.Features.Students.DTOs;

/// <summary>
/// DTO for updating students
/// </summary>
public class UpdateStudentDto
{
    /// <summary>
    /// Student full name
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// Student email address
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Student phone number
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Student address
    /// </summary>
    public AddressDto? Address { get; set; }

    /// <summary>
    /// Student GPA
    /// </summary>
    public decimal? GPA { get; set; }
}
