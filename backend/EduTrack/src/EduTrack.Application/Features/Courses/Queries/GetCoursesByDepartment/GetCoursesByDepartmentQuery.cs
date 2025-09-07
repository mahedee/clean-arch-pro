using MediatR;
using EduTrack.Application.Features.Courses.DTOs;

namespace EduTrack.Application.Features.Courses.Queries.GetCoursesByDepartment;

/// <summary>
/// Query to get courses by department
/// </summary>
public class GetCoursesByDepartmentQuery : IRequest<List<CourseListDto>>
{
    /// <summary>
    /// Department name
    /// </summary>
    public string Department { get; set; } = string.Empty;

    /// <summary>
    /// Optional course level filter
    /// </summary>
    public string? Level { get; set; }

    /// <summary>
    /// Optional status filter
    /// </summary>
    public string? Status { get; set; }

    public GetCoursesByDepartmentQuery(string department)
    {
        Department = department;
    }
}
