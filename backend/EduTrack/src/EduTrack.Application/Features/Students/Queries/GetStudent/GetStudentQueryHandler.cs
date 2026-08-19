using AutoMapper;
using MediatR;
using EduTrack.Application.Features.Students.DTOs;
using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Domain.Common.Exceptions;

namespace EduTrack.Application.Features.Students.Queries.GetStudent;

/// <summary>
/// Handler for GetStudentQuery
/// </summary>
public class GetStudentQueryHandler : IRequestHandler<GetStudentQuery, StudentDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetStudentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<StudentDto?> Handle(GetStudentQuery request, CancellationToken cancellationToken)
    {
        var student = await _unitOfWork.Students.GetByIdAsync(request.StudentId, cancellationToken);
        
        if (student == null)
            throw new EntityNotFoundException(nameof(student), request.StudentId);
            
        return _mapper.Map<StudentDto>(student);
    }
}
