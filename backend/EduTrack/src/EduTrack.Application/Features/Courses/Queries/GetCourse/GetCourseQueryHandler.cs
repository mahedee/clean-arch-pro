using AutoMapper;
using MediatR;
using EduTrack.Application.Features.Courses.DTOs;
using EduTrack.Domain.Repositories;

namespace EduTrack.Application.Features.Courses.Queries.GetCourse;

/// <summary>
/// Handler for GetCourseQuery
/// </summary>
public class GetCourseQueryHandler : IRequestHandler<GetCourseQuery, CourseDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCourseQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CourseDto?> Handle(GetCourseQuery request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId);
        
        if (course == null)
            return null;
            
        return _mapper.Map<CourseDto>(course);
    }
}
