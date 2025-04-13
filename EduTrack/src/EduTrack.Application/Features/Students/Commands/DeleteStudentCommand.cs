using MediatR;

namespace EduTrack.Application.Features.Students.Commands
{
    public class DeleteStudentCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
