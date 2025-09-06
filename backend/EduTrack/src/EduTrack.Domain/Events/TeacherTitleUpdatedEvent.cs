using EduTrack.Domain.Common;
using EduTrack.Domain.Entities;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when teacher's academic title is updated
    /// </summary>
    public class TeacherTitleUpdatedEvent : DomainEvent
    {
        public Guid TeacherId { get; }
        public string TeacherName { get; }
        public AcademicTitle NewTitle { get; }
        public AcademicTitle PreviousTitle { get; }

        public TeacherTitleUpdatedEvent(Guid teacherId, string teacherName, AcademicTitle newTitle, AcademicTitle previousTitle)
        {
            TeacherId = teacherId;
            TeacherName = teacherName;
            NewTitle = newTitle;
            PreviousTitle = previousTitle;
        }
    }
}
