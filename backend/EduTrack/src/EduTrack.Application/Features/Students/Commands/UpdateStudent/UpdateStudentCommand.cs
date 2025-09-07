using MediatR;

namespace EduTrack.Application.Features.Students.Commands.UpdateStudent;

/// <summary>
/// Command to update an existing student
/// </summary>
public record UpdateStudentCommand(
    Guid Id,
    string? FullName = null,
    string? Email = null,
    string? PhoneNumber = null,
    string? Street = null,
    string? City = null,
    string? State = null,
    string? ZipCode = null,
    string? Country = null,
    decimal? GPA = null
) : IRequest<Unit>;