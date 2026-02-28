using MediatR;

namespace EduTrack.Application.Features.Courses.Commands.DeleteCourse;

/// <summary>
/// Command to delete an existing course
/// </summary>
public class DeleteCourseCommand : IRequest<bool>
{
    /// <summary>
    /// Course ID to delete
    /// </summary>
    public Guid CourseId { get; set; }
}