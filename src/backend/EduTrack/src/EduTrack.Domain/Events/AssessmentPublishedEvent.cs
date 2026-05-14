using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when an assessment is published for students
    /// </summary>
    public class AssessmentPublishedEvent : DomainEvent
    {
        public Guid AssessmentId { get; }
        public Guid CourseId { get; }
        public string Title { get; }
        public DateTime DueDate { get; }

        public AssessmentPublishedEvent(Guid assessmentId, Guid courseId, string title, DateTime dueDate)
        {
            AssessmentId = assessmentId;
            CourseId = courseId;
            Title = title;
            DueDate = dueDate;
        }
    }
}
