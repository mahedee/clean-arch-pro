using MediatR;

namespace EduTrack.Application.Features.Teachers.Commands.DeleteTeacher;

public record DeleteTeacherCommand(Guid TeacherId) : IRequest;
