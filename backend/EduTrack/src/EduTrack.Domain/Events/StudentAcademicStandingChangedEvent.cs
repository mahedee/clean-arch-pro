using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a student's academic standing changes (e.g., goes on probation, dean's list)
    /// </summary>
    public class StudentAcademicStandingChangedEvent : DomainEvent
    {
        public Guid StudentId { get; }
        public string FullName { get; }
        public string Email { get; }
        public decimal CurrentGPA { get; }
        public string PreviousStanding { get; }
        public string NewStanding { get; }
        public string? Reason { get; }
        public DateTime EffectiveDate { get; }

        public StudentAcademicStandingChangedEvent(
            Guid studentId,
            string fullName,
            string email,
            decimal currentGPA,
            string previousStanding,
            string newStanding,
            string? reason = null)
        {
            StudentId = studentId;
            FullName = fullName;
            Email = email;
            CurrentGPA = currentGPA;
            PreviousStanding = previousStanding;
            NewStanding = newStanding;
            Reason = reason;
            EffectiveDate = DateTime.UtcNow;
        }
    }
}
