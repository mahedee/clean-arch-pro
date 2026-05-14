using AutoMapper;
using EduTrack.Application.Features.Departments.DTOs;
using EduTrack.Domain.Contracts.Repositories;
using MediatR;

namespace EduTrack.Application.Features.Departments.Queries.GetDepartment;

public class GetDepartmentQueryHandler : IRequestHandler<GetDepartmentQuery, DepartmentDto?>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetDepartmentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<DepartmentDto?> Handle(GetDepartmentQuery request, CancellationToken cancellationToken)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(request.DepartmentId, cancellationToken);
        return department is null ? null : _mapper.Map<DepartmentDto>(department);
    }
}
