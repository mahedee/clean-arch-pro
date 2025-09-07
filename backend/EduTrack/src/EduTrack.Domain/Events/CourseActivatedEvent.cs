using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a course is activated
    /// </summary>
    public class CourseActivatedEvent : DomainEvent
    {
        public Guid CourseId { get; }
        public string Title { get; }
        public string Code { get; }

        public CourseActivatedEvent(Guid courseId, string title, string code)
        {
            CourseId = courseId;
            Title = title;
            Code = code;
        }
    }
}
