using MediatR;

namespace EduTrack.Application.Features.Teachers.Commands.CreateTeacher;

public record CreateTeacherCommand : IRequest<Guid>
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string EmployeeId { get; init; } = string.Empty;
    public string Department { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
}
