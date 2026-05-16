using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a student withdraws from an enrollment
    /// </summary>
    public class EnrollmentWithdrawnEvent : DomainEvent
    {
        public Guid EnrollmentId { get; }
        public Guid StudentId { get; }
        public Guid CourseId { get; }
        public string? Reason { get; }

        public EnrollmentWithdrawnEvent(Guid enrollmentId, Guid studentId, Guid courseId, string? reason)
        {
            EnrollmentId = enrollmentId;
            StudentId = studentId;
            CourseId = courseId;
            Reason = reason;
        }
    }
}
