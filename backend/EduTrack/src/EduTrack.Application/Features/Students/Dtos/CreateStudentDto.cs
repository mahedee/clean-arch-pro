namespace EduTrack.Application.Features.Students.DTOs;

/// <summary>
/// DTO for creating students
/// </summary>
public class CreateStudentDto
{
    /// <summary>
    /// Student full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Student date of birth
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Student email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Student phone number (optional)
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Student address (optional)
    /// </summary>
    public AddressDto? Address { get; set; }
}
