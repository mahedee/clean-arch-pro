using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a new course is created
    /// </summary>
    public class CourseCreatedEvent : DomainEvent
    {
        public Guid CourseId { get; }
        public string Title { get; }
        public string Code { get; }
        public string Department { get; }

        public CourseCreatedEvent(Guid courseId, string title, string code, string department)
        {
            CourseId = courseId;
            Title = title;
            Code = code;
            Department = department;
        }
    }
}
