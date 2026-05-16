using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a student enrollment is created
    /// </summary>
    public class EnrollmentCreatedEvent : DomainEvent
    {
        public Guid EnrollmentId { get; }
        public Guid StudentId { get; }
        public Guid CourseId { get; }
        public string Semester { get; }
        public int AcademicYear { get; }

        public EnrollmentCreatedEvent(Guid enrollmentId, Guid studentId, Guid courseId, string semester, int academicYear)
        {
            EnrollmentId = enrollmentId;
            StudentId = studentId;
            CourseId = courseId;
            Semester = semester;
            AcademicYear = academicYear;
        }
    }
}
