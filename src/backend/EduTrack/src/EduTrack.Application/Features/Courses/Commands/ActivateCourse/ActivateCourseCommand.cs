using MediatR;

namespace EduTrack.Application.Features.Courses.Commands.ActivateCourse;

/// <summary>
/// Command to activate a course (make it available for enrollment)
/// </summary>
public class ActivateCourseCommand : IRequest<bool>
{
    /// <summary>
    /// Course ID to activate
    /// </summary>
    public Guid CourseId { get; set; }
}
