using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when course information is updated
    /// </summary>
    public class CourseUpdatedEvent : DomainEvent
    {
        public Guid CourseId { get; }
        public string Title { get; }
        public string Code { get; }

        public CourseUpdatedEvent(Guid courseId, string title, string code)
        {
            CourseId = courseId;
            Title = title;
            Code = code;
        }
    }
}
