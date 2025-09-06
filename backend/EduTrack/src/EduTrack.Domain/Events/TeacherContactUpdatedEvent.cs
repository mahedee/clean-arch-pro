using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when teacher contact information is updated
    /// </summary>
    public class TeacherContactUpdatedEvent : DomainEvent
    {
        public Guid TeacherId { get; }
        public string NewEmail { get; }
        public string PreviousEmail { get; }

        public TeacherContactUpdatedEvent(Guid teacherId, string newEmail, string previousEmail)
        {
            TeacherId = teacherId;
            NewEmail = newEmail;
            PreviousEmail = previousEmail;
        }
    }
}
