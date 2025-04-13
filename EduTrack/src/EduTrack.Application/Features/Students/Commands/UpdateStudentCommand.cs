using MediatR;

// You can use namespace as below or use the default namespace
namespace EduTrack.Application.Features.Students.Commands;
public class UpdateStudentCommand : IRequest<bool>
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string? Email { get; set; }
}
