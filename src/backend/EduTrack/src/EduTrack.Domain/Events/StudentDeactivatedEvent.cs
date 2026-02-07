using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a student is deactivated (soft delete)
    /// </summary>
    public class StudentDeactivatedEvent : DomainEvent
    {
        public Guid StudentId { get; }
        public string FullName { get; }
        public string Email { get; }
        public string? Reason { get; }
        public string? DeactivatedBy { get; }
        public DateTime DeactivatedAt { get; }

        public StudentDeactivatedEvent(
            Guid studentId,
            string fullName,
            string email,
            string? reason = null,
            string? deactivatedBy = null)
        {
            StudentId = studentId;
            FullName = fullName;
            Email = email;
            Reason = reason;
            DeactivatedBy = deactivatedBy;
            DeactivatedAt = DateTime.UtcNow;
        }
    }
}
