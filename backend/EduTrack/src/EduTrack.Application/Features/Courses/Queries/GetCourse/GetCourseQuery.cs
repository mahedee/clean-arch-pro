using MediatR;
using EduTrack.Application.Features.Courses.DTOs;

namespace EduTrack.Application.Features.Courses.Queries.GetCourse;

/// <summary>
/// Query to get a single course by ID
/// </summary>
public class GetCourseQuery : IRequest<CourseDto?>
{
    /// <summary>
    /// Course ID to retrieve
    /// </summary>
    public Guid CourseId { get; set; }

    public GetCourseQuery(Guid courseId)
    {
        CourseId = courseId;
    }
}
