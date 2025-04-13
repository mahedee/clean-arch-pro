using EduTrack.Application.Features.Students.Dtos;
using MediatR;

namespace EduTrack.Application.Features.Students.Queries
{
    public class GetStudentByIdQuery : IRequest<StudentDto>
    {
        public Guid Id { get; set; }
    }
}
