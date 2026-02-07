using MediatR;
using EduTrack.Application.Features.Students.DTOs;
using EduTrack.Domain.Entities;

namespace EduTrack.Application.Features.Students.Queries.GetStudentsByStatus;

/// <summary>
/// Query to get students by status with pagination
/// </summary>
public class GetStudentsByStatusQuery : IRequest<PaginatedStudentListDto>
{
    /// <summary>
    /// Student status to filter by
    /// </summary>
    public StudentStatus Status { get; set; }

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Page size (number of items per page)
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Sort by field (FullName, Email, DateOfBirth, etc.)
    /// </summary>
    public string SortBy { get; set; } = "FullName";

    /// <summary>
    /// Sort direction (asc, desc)
    /// </summary>
    public string SortDirection { get; set; } = "asc";

    public GetStudentsByStatusQuery(StudentStatus status)
    {
        Status = status;
    }
}