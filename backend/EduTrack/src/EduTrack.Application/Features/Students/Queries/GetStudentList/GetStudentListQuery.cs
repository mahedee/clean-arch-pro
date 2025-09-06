using MediatR;
using EduTrack.Application.Features.Students.DTOs;
using EduTrack.Domain.Entities;

namespace EduTrack.Application.Features.Students.Queries.GetStudentList;

/// <summary>
/// Query to get paginated list of students
/// </summary>
public class GetStudentListQuery : IRequest<PaginatedStudentListDto>
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
    /// Optional search term for student name or email
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Optional status filter
    /// </summary>
    public StudentStatus? Status { get; set; }

    /// <summary>
    /// Sort by field (FullName, Email, DateOfBirth, etc.)
    /// </summary>
    public string SortBy { get; set; } = "FullName";

    /// <summary>
    /// Sort direction (asc, desc)
    /// </summary>
    public string SortDirection { get; set; } = "asc";

    /// <summary>
    /// Minimum GPA filter
    /// </summary>
    public decimal? MinGPA { get; set; }

    /// <summary>
    /// Maximum GPA filter
    /// </summary>
    public decimal? MaxGPA { get; set; }

    /// <summary>
    /// Minimum age filter
    /// </summary>
    public int? MinAge { get; set; }

    /// <summary>
    /// Maximum age filter
    /// </summary>
    public int? MaxAge { get; set; }
}