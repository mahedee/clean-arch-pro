using AutoMapper;
using MediatR;
using EduTrack.Application.Features.Students.DTOs;
using EduTrack.Domain.Contracts.Repositories;

namespace EduTrack.Application.Features.Students.Queries.GetStudentsByStatus;

/// <summary>
/// Handler for GetStudentsByStatusQuery
/// </summary>
public class GetStudentsByStatusQueryHandler : IRequestHandler<GetStudentsByStatusQuery, PaginatedStudentListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetStudentsByStatusQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedStudentListDto> Handle(GetStudentsByStatusQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get students by status from repository
            var (students, totalCount) = await _unitOfWork.Students.GetByStatusAsync(
                status: request.Status,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
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
            throw new InvalidOperationException($"Error retrieving students with status {request.Status}", ex);
        }
    }
}
