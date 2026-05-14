using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when an enrollment is completed with a final grade
    /// </summary>
    public class EnrollmentCompletedEvent : DomainEvent
    {
        public Guid EnrollmentId { get; }
        public Guid StudentId { get; }
        public Guid CourseId { get; }
        public decimal FinalGrade { get; }

        public EnrollmentCompletedEvent(Guid enrollmentId, Guid studentId, Guid courseId, decimal finalGrade)
        {
            EnrollmentId = enrollmentId;
            StudentId = studentId;
            CourseId = courseId;
            FinalGrade = finalGrade;
        }
    }
}
