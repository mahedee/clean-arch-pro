// UpdateStudentGPA/UpdateStudentGPACommand.cs
using MediatR;

public record UpdateStudentGPACommand(
    Guid Id,
    decimal GPAValue
) : IRequest<Unit>;