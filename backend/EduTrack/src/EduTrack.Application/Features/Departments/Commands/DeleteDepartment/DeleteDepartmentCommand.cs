using MediatR;

namespace EduTrack.Application.Features.Departments.Commands.DeleteDepartment;

public record DeleteDepartmentCommand(Guid DepartmentId) : IRequest;
