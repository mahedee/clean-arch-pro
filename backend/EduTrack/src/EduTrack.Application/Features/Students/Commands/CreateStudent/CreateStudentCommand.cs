using MediatR;

namespace EduTrack.Application.Features.Students.Commands.CreateStudent;

/// <summary>
/// Command to create a new student
/// </summary>
public record CreateStudentCommand(
    string FullName,
    DateTime DateOfBirth,
    string Email,
    string? PhoneNumber = null,
    string? Street = null,
    string? City = null,
    string? State = null,
    string? ZipCode = null,
    string? Country = null
) : IRequest<Guid>;