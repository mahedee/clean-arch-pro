using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when student contact information is updated
    /// </summary>
    public class StudentContactUpdatedEvent : DomainEvent
    {
        public Guid StudentId { get; }
        public string NewEmail { get; }
        public string? PreviousEmail { get; }

        public StudentContactUpdatedEvent(Guid studentId, string newEmail, string? previousEmail = null)
        {
            StudentId = studentId;
            NewEmail = newEmail;
            PreviousEmail = previousEmail;
        }
    }
}
