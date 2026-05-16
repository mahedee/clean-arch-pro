namespace EduTrack.Application.Features.Students.DTOs;

/// <summary>
/// DTO for updating student contact information
/// </summary>
public class UpdateStudentContactDto
{
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
}
