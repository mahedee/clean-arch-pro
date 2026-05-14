using EduTrack.Application.Features.Departments.DTOs;
using MediatR;

namespace EduTrack.Application.Features.Departments.Queries.GetDepartmentList;

public record GetDepartmentListQuery : IRequest<PaginatedDepartmentListDto>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public string? Status { get; init; }
    public string SortBy { get; init; } = "Name";
    public string SortDirection { get; init; } = "asc";
}
