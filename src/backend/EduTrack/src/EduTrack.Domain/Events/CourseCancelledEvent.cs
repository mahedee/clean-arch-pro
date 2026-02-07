using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a course is cancelled
    /// </summary>
    public class CourseCancelledEvent : DomainEvent
    {
        public Guid CourseId { get; }
        public string Title { get; }
        public string Code { get; }
        public string Reason { get; }

        public CourseCancelledEvent(Guid courseId, string title, string code, string reason)
        {
            CourseId = courseId;
            Title = title;
            Code = code;
            Reason = reason;
        }
    }
}
