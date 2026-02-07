using EduTrack.Domain.Common;
using EduTrack.Domain.Entities;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a teacher is deactivated
    /// </summary>
    public class TeacherDeactivatedEvent : DomainEvent
    {
        public Guid TeacherId { get; }
        public string TeacherName { get; }
        public EmploymentStatus NewStatus { get; }

        public TeacherDeactivatedEvent(Guid teacherId, string teacherName, EmploymentStatus newStatus)
        {
            TeacherId = teacherId;
            TeacherName = teacherName;
            NewStatus = newStatus;
        }
    }
}
