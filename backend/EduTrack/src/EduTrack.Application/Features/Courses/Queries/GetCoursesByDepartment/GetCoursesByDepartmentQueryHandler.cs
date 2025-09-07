using AutoMapper;
using MediatR;
using EduTrack.Application.Features.Courses.DTOs;
using EduTrack.Domain.Repositories;
using EduTrack.Domain.Entities;

namespace EduTrack.Application.Features.Courses.Queries.GetCoursesByDepartment;

/// <summary>
/// Handler for GetCoursesByDepartmentQuery
/// </summary>
public class GetCoursesByDepartmentQueryHandler : IRequestHandler<GetCoursesByDepartmentQuery, List<CourseListDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCoursesByDepartmentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<CourseListDto>> Handle(GetCoursesByDepartmentQuery request, CancellationToken cancellationToken)
    {
        var courses = await _unitOfWork.Courses.GetByDepartmentAsync(request.Department);

        // Apply additional filters if provided
        if (!string.IsNullOrWhiteSpace(request.Level) && Enum.TryParse<CourseLevel>(request.Level, true, out var courseLevel))
        {
            courses = courses.Where(c => c.Level == courseLevel);
        }

        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<CourseStatus>(request.Status, true, out var courseStatus))
        {
            courses = courses.Where(c => c.Status == courseStatus);
        }

        // Sort by title
        courses = courses.OrderBy(c => c.Title);

        return _mapper.Map<List<CourseListDto>>(courses.ToList());
    }
}
