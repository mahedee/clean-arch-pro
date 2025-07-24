using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a new student is created
    /// </summary>
    public class StudentCreatedEvent : DomainEvent
    {
        public Guid StudentId { get; }
        public string FullName { get; }
        public string Email { get; }

        public StudentCreatedEvent(Guid studentId, string fullName, string email)
        {
            StudentId = studentId;
            FullName = fullName;
            Email = email;
        }
    }
}
