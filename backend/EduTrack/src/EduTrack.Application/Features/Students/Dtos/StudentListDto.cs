namespace EduTrack.Application.Features.Students.DTOs;

/// <summary>
/// Simplified student DTO for list views
/// </summary>
public class StudentListDto
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
    /// Student email address
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Student age
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// Student phone number
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Student GPA
    /// </summary>
    public decimal? GPA { get; set; }

    /// <summary>
    /// Student status
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Student enrollment date
    /// </summary>
    public DateTime EnrollmentDate { get; set; }
}
