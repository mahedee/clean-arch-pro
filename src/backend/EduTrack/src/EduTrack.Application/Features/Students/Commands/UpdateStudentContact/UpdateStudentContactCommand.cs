// UpdateStudentContact/UpdateStudentContactCommand.cs
using MediatR;

public record UpdateStudentContactCommand(
    Guid Id,
    string Email,
    string? PhoneNumber = null
) : IRequest<Unit>;