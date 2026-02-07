using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a course is completed
    /// </summary>
    public class CourseCompletedEvent : DomainEvent
    {
        public Guid CourseId { get; }
        public string Title { get; }
        public string Code { get; }
        public int FinalEnrollment { get; }

        public CourseCompletedEvent(Guid courseId, string title, string code, int finalEnrollment)
        {
            CourseId = courseId;
            Title = title;
            Code = code;
            FinalEnrollment = finalEnrollment;
        }
    }
}
