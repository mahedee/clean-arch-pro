using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a student's GPA is updated
    /// </summary>
    public class StudentGPAUpdatedEvent : DomainEvent
    {
        public Guid StudentId { get; }
        public string FullName { get; }
        public string Email { get; }
        public decimal? PreviousGPA { get; }
        public decimal NewGPA { get; }
        public bool IsFirstGPA { get; }
        public string? UpdateReason { get; }

        public StudentGPAUpdatedEvent(
            Guid studentId,
            string fullName,
            string email,
            decimal? previousGPA,
            decimal newGPA,
            string? updateReason = null)
        {
            StudentId = studentId;
            FullName = fullName;
            Email = email;
            PreviousGPA = previousGPA;
            NewGPA = newGPA;
            IsFirstGPA = previousGPA == null;
            UpdateReason = updateReason;
        }
    }
}
