using EduTrack.Domain.Common;
using EduTrack.Domain.Entities;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a student's status changes
    /// </summary>
    public class StudentStatusChangedEvent : DomainEvent
    {
        public Guid StudentId { get; }
        public string FullName { get; }
        public string Email { get; }
        public StudentStatus PreviousStatus { get; }
        public StudentStatus NewStatus { get; }
        public string? Reason { get; }

        public StudentStatusChangedEvent(
            Guid studentId, 
            string fullName, 
            string email, 
            StudentStatus previousStatus, 
            StudentStatus newStatus, 
            string? reason = null)
        {
            StudentId = studentId;
            FullName = fullName;
            Email = email;
            PreviousStatus = previousStatus;
            NewStatus = newStatus;
            Reason = reason;
        }
    }
}
