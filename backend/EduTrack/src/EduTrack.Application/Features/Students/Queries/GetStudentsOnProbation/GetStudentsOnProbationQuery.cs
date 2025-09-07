using MediatR;
using EduTrack.Application.Features.Students.DTOs;

namespace EduTrack.Application.Features.Students.Queries.GetStudentsOnProbation;

/// <summary>
/// Query to get students on academic probation (GPA below threshold)
/// </summary>
public class GetStudentsOnProbationQuery : IRequest<PaginatedStudentListDto>
{
    /// <summary>
    /// GPA threshold for probation (students below this GPA)
    /// </summary>
    public decimal GPAThreshold { get; set; } = 2.0m;

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Page size (number of items per page)
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Sort by field (FullName, Email, GPA, etc.)
    /// </summary>
    public string SortBy { get; set; } = "GPA";

    /// <summary>
    /// Sort direction (asc, desc)
    /// </summary>
    public string SortDirection { get; set; } = "asc";
}