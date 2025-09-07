namespace EduTrack.Application.Features.Students.DTOs;

/// <summary>
/// DTO for updating student GPA
/// </summary>
public class UpdateGPADto
{
    /// <summary>
    /// New GPA value
    /// </summary>
    public decimal GPAValue { get; set; }
}
