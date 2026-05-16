using AutoMapper;
using EduTrack.Application.Features.Teachers.DTOs;
using EduTrack.Domain.Contracts.Repositories;
using EduTrack.Domain.Enums;
using MediatR;

namespace EduTrack.Application.Features.Teachers.Queries.GetTeacherList;

public class GetTeacherListQueryHandler : IRequestHandler<GetTeacherListQuery, PaginatedTeacherListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetTeacherListQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedTeacherListDto> Handle(GetTeacherListQuery request, CancellationToken cancellationToken)
    {
        EmploymentStatus? status = null;
        if (!string.IsNullOrWhiteSpace(request.Status) && Enum.TryParse<EmploymentStatus>(request.Status, out var parsedStatus))
            status = parsedStatus;

        var (teachers, totalCount) = await _unitOfWork.Teachers.GetPagedAsync(
            request.PageNumber,
            request.PageSize,
            request.SearchTerm,
            request.Department,
            status,
            request.SortBy,
            request.SortDirection?.ToLower() == "desc",
            cancellationToken);

        return new PaginatedTeacherListDto
        {
            Teachers = _mapper.Map<List<TeacherListDto>>(teachers),
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}
