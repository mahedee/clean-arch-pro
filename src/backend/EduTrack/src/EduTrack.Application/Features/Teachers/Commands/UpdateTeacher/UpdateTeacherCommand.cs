using MediatR;

namespace EduTrack.Application.Features.Teachers.Commands.UpdateTeacher;

public record UpdateTeacherCommand : IRequest<Unit>
{
    public Guid Id { get; init; }
    public string? FullName { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Department { get; init; }
    public string? Title { get; init; }
    public string? OfficeLocation { get; init; }
    public string? OfficeHours { get; init; }
}
