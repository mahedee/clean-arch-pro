using MediatR;

namespace EduTrack.Application.Features.Courses.Commands.CompleteCourse;

/// <summary>
/// Command to mark a course as completed
/// </summary>
public class CompleteCourseCommand : IRequest<bool>
{
    /// <summary>
    /// Course ID to complete
    /// </summary>
    public Guid CourseId { get; set; }

    /// <summary>
    /// Optional completion notes
    /// </summary>
    public string? CompletionNotes { get; set; }
}
