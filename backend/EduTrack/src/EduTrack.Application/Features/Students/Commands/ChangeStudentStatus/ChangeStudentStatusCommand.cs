using MediatR;
using EduTrack.Domain.Entities;

namespace EduTrack.Application.Features.Students.Commands.ChangeStudentStatus;

public record ChangeStudentStatusCommand(
    Guid Id,
    StudentStatus NewStatus
) : IRequest<Unit>;