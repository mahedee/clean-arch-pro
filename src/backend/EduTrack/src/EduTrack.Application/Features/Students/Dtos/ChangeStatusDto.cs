namespace EduTrack.Application.Features.Students.DTOs;

/// <summary>
/// DTO for changing student status
/// </summary>
public class ChangeStatusDto
{
    /// <summary>
    /// New student status
    /// </summary>
    public string NewStatus { get; set; } = string.Empty;
}
