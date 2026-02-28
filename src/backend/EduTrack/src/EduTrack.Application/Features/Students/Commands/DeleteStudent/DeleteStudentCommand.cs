using MediatR;

namespace EduTrack.Application.Features.Students.Commands.DeleteStudent;

/// <summary>
/// Command to delete a student
/// </summary>
public record DeleteStudentCommand(
    Guid StudentId
) : IRequest;
