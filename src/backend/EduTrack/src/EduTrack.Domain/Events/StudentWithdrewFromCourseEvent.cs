using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a student withdraws from a course
    /// </summary>
    public class StudentWithdrewFromCourseEvent : DomainEvent
    {
        public Guid CourseId { get; }
        public string CourseTitle { get; }
        public string CourseCode { get; }
        public int CurrentEnrollment { get; }
        public int MaxEnrollment { get; }

        public StudentWithdrewFromCourseEvent(Guid courseId, string courseTitle, string courseCode, int currentEnrollment, int maxEnrollment)
        {
            CourseId = courseId;
            CourseTitle = courseTitle;
            CourseCode = courseCode;
            CurrentEnrollment = currentEnrollment;
            MaxEnrollment = maxEnrollment;
        }
    }
}
