using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a student's personal information is updated
    /// </summary>
    public class StudentPersonalInfoUpdatedEvent : DomainEvent
    {
        public Guid StudentId { get; }
        public string? PreviousFullName { get; }
        public string? NewFullName { get; }
        public DateTime? PreviousDateOfBirth { get; }
        public DateTime? NewDateOfBirth { get; }
        public string? UpdatedBy { get; }

        public StudentPersonalInfoUpdatedEvent(
            Guid studentId,
            string? previousFullName,
            string? newFullName,
            DateTime? previousDateOfBirth = null,
            DateTime? newDateOfBirth = null,
            string? updatedBy = null)
        {
            StudentId = studentId;
            PreviousFullName = previousFullName;
            NewFullName = newFullName;
            PreviousDateOfBirth = previousDateOfBirth;
            NewDateOfBirth = newDateOfBirth;
            UpdatedBy = updatedBy;
        }
    }
}
