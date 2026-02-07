using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a teacher is reactivated
    /// </summary>
    public class TeacherReactivatedEvent : DomainEvent
    {
        public Guid TeacherId { get; }
        public string TeacherName { get; }

        public TeacherReactivatedEvent(Guid teacherId, string teacherName)
        {
            TeacherId = teacherId;
            TeacherName = teacherName;
        }
    }
}
