using AutoMapper;
using EduTrack.Application.Features.Departments.DTOs;
using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Domain.Enums;
using MediatR;

namespace EduTrack.Application.Features.Departments.Queries.GetDepartmentList;

public class GetDepartmentListQueryHandler : IRequestHandler<GetDepartmentListQuery, PaginatedDepartmentListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetDepartmentListQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedDepartmentListDto> Handle(GetDepartmentListQuery request, CancellationToken cancellationToken)
    {
        DepartmentStatus? status = null;
        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<DepartmentStatus>(request.Status, out var parsedStatus))
            status = parsedStatus;

        var (departments, totalCount) = await _unitOfWork.Departments.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.SearchTerm,
            status,
            request.SortBy,
            request.SortDirection?.ToLower() == "desc",
            cancellationToken);

        return new PaginatedDepartmentListDto
        {
            Departments = _mapper.Map<List<DepartmentListDto>>(departments),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
