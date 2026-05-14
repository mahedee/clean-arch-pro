using EduTrack.Application.Features.Teachers.DTOs;
using MediatR;

namespace EduTrack.Application.Features.Teachers.Queries.GetTeacherList;

public record GetTeacherListQuery : IRequest<PaginatedTeacherListDto>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public string? Department { get; init; }
    public string? Status { get; init; }
    public string SortBy { get; init; } = "FullName";
    public string SortDirection { get; init; } = "asc";
}
