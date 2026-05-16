using AutoMapper;
using EduTrack.Application.Features.Teachers.DTOs;
using EduTrack.Domain.Contracts.Repositories;
using MediatR;

namespace EduTrack.Application.Features.Teachers.Queries.GetTeacher;

public class GetTeacherQueryHandler : IRequestHandler<GetTeacherQuery, TeacherDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTeacherQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<TeacherDto?> Handle(GetTeacherQuery request, CancellationToken cancellationToken)
    {
        var teacher = await _unitOfWork.Teachers.GetByIdAsync(request.TeacherId, cancellationToken);
        return teacher is null ? null : _mapper.Map<TeacherDto>(teacher);
    }
}
