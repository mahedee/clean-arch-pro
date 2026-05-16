using MediatR;

namespace EduTrack.Application.Features.Departments.Commands.CreateDepartment;

public record CreateDepartmentCommand : IRequest<Guid>
{
    public string Name { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Location { get; init; }
    public string? ContactEmail { get; init; }
    public string? ContactPhone { get; init; }
}
