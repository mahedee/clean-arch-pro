using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a course is scheduled
    /// </summary>
    public class CourseScheduledEvent : DomainEvent
    {
        public Guid CourseId { get; }
        public string Semester { get; }
        public int AcademicYear { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }

        public CourseScheduledEvent(Guid courseId, string semester, int academicYear, DateTime startDate, DateTime endDate)
        {
            CourseId = courseId;
            Semester = semester;
            AcademicYear = academicYear;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
