using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a student is reactivated
    /// </summary>
    public class StudentReactivatedEvent : DomainEvent
    {
        public Guid StudentId { get; }
        public string FullName { get; }
        public string Email { get; }
        public string? Reason { get; }
        public string? ReactivatedBy { get; }
        public DateTime ReactivatedAt { get; }

        public StudentReactivatedEvent(
            Guid studentId,
            string fullName,
            string email,
            string? reason = null,
            string? reactivatedBy = null)
        {
            StudentId = studentId;
            FullName = fullName;
            Email = email;
            Reason = reason;
            ReactivatedBy = reactivatedBy;
            ReactivatedAt = DateTime.UtcNow;
        }
    }
}
