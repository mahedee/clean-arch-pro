using AutoMapper;
using MediatR;
using EduTrack.Application.Features.Courses.DTOs;
using EduTrack.Domain.Repositories;

namespace EduTrack.Application.Features.Courses.Queries.GetCourseList;

/// <summary>
/// Handler for GetCourseListQuery
/// </summary>
public class GetCourseListQueryHandler : IRequestHandler<GetCourseListQuery, PaginatedCourseListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCourseListQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedCourseListDto> Handle(GetCourseListQuery request, CancellationToken cancellationToken)
    {
        var (courses, totalCount) = await _unitOfWork.Courses.GetPaginatedAsync(
            request.PageNumber,
            request.PageSize,
            request.SearchTerm,
            request.Department,
            request.Level,
            request.Status,
            request.SortBy,
            request.SortDirection);

        var courseDtos = _mapper.Map<List<CourseListDto>>(courses);

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        return new PaginatedCourseListDto
        {
            Courses = courseDtos,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasNextPage = request.PageNumber < totalPages,
            HasPreviousPage = request.PageNumber > 1
        };
    }
}
