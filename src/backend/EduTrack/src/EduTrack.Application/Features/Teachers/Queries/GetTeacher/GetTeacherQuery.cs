using EduTrack.Application.Features.Teachers.DTOs;
using MediatR;

namespace EduTrack.Application.Features.Teachers.Queries.GetTeacher;

public record GetTeacherQuery(Guid TeacherId) : IRequest<TeacherDto?>;
