using MediatR;

namespace EduTrack.Application.Features.Departments.Commands.UpdateDepartment;

public record UpdateDepartmentCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public string? Location { get; init; }
    public string? ContactEmail { get; init; }
    public string? ContactPhone { get; init; }
}
