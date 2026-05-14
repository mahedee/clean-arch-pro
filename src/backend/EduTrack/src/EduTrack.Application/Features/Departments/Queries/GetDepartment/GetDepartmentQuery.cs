using EduTrack.Application.Features.Departments.DTOs;
using MediatR;

namespace EduTrack.Application.Features.Departments.Queries.GetDepartment;

public record GetDepartmentQuery(Guid DepartmentId) : IRequest<DepartmentDto?>;
