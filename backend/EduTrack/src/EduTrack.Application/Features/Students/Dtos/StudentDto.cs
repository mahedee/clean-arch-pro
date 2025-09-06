namespace EduTrack.Application.Features.Students.DTOs;

/// <summary>
/// Student data transfer object for API responses
/// </summary>
public class StudentDto
{
    /// <summary>
    /// Student unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Student full name
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Student date of birth
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// Student age (calculated)
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// Student email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

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

    /// <summary>
    /// Student enrollment status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Student enrollment date
    /// </summary>
    public DateTime EnrollmentDate { get; set; }

    /// <summary>
    /// Record creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Record last update timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
