using AutoMapper;
using MediatR;
using EduTrack.Application.Features.Students.DTOs;
using EduTrack.Domain.Contracts.Repositories;

namespace EduTrack.Application.Features.Students.Queries.GetStudentsOnProbation;

/// <summary>
/// Handler for GetStudentsOnProbationQuery
/// </summary>
public class GetStudentsOnProbationQueryHandler : IRequestHandler<GetStudentsOnProbationQuery, PaginatedStudentListDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetStudentsOnProbationQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedStudentListDto> Handle(GetStudentsOnProbationQuery request, CancellationToken cancellationToken)
    {
        try
        {
            // Get students with GPA below threshold
            var (students, totalCount) = await _unitOfWork.Students.GetPagedAsync(
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                searchTerm: null,
                status: null,
                sortBy: request.SortBy,
                sortDescending: request.SortDirection.ToLower() == "desc",
                cancellationToken: cancellationToken);

            // Filter students with GPA below threshold (students on probation)
            var studentsOnProbation = students.Where(s => 
                s.CurrentGPA != null && s.CurrentGPA.Value < request.GPAThreshold).ToList();

            var probationCount = studentsOnProbation.Count;

            // Apply pagination to filtered results
            var pagedStudents = studentsOnProbation
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            // Map to DTOs
            var studentDtos = _mapper.Map<List<StudentListDto>>(pagedStudents);

            // Calculate pagination metadata
            var totalPages = (int)Math.Ceiling((double)probationCount / request.PageSize);
            var hasNextPage = request.PageNumber < totalPages;
            var hasPreviousPage = request.PageNumber > 1;

            return new PaginatedStudentListDto
            {
                Students = studentDtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = probationCount,
                TotalPages = totalPages,
                HasPreviousPage = hasPreviousPage,
                HasNextPage = hasNextPage
            };
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error retrieving students on probation", ex);
        }
    }
}
