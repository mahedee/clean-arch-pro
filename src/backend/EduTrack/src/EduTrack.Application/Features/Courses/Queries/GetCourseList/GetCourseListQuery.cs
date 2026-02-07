using MediatR;
using EduTrack.Application.Features.Courses.DTOs;

namespace EduTrack.Application.Features.Courses.Queries.GetCourseList;

/// <summary>
/// Query to get paginated list of courses
/// </summary>
public class GetCourseListQuery : IRequest<PaginatedCourseListDto>
{
    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Page size (number of items per page)
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Optional search term for course title or code
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Optional department filter
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// Optional level filter
    /// </summary>
    public string? Level { get; set; }

    /// <summary>
    /// Optional status filter
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Sort by field (Title, Code, Department, etc.)
    /// </summary>
    public string SortBy { get; set; } = "Title";

    /// <summary>
    /// Sort direction (asc, desc)
    /// </summary>
    public string SortDirection { get; set; } = "asc";
}
