using AutoMapper;
using MediatR;
using EduTrack.Application.Features.Students.DTOs;
using EduTrack.Domain.Repositories;

namespace EduTrack.Application.Features.Students.Queries.GetStudentList;

/// <summary>
/// Handler for GetStudentListQuery
/// </summary>
public class GetStudentListQueryHandler : IRequestHandler<GetStudentListQuery, PaginatedStudentListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetStudentListQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedStudentListDto> Handle(GetStudentListQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get paginated students from repository
            var (students, totalCount) = await _unitOfWork.Students.GetPagedAsync(
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                searchTerm: request.SearchTerm,
                status: request.Status,
                sortBy: request.SortBy,
                sortDescending: request.SortDirection.ToLower() == "desc",
                cancellationToken: cancellationToken);

            // Map to DTOs
            var studentDtos = _mapper.Map<List<StudentListDto>>(students);

            // Calculate pagination metadata
            var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
            var hasNextPage = request.PageNumber < totalPages;
            var hasPreviousPage = request.PageNumber > 1;

            return new PaginatedStudentListDto
            {
                Students = studentDtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasPreviousPage = hasPreviousPage,
                HasNextPage = hasNextPage
            };
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error retrieving student list", ex);
        }
    }
}
