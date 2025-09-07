using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a teacher is removed from a course
    /// </summary>
    public class TeacherRemovedFromCourseEvent : DomainEvent
    {
        public Guid TeacherId { get; }
        public string TeacherName { get; }
        public Guid CourseId { get; }
        public string CourseName { get; }
        public int CurrentCourseLoad { get; }

        public TeacherRemovedFromCourseEvent(Guid teacherId, string teacherName, Guid courseId, string courseName, int currentCourseLoad)
        {
            TeacherId = teacherId;
            TeacherName = teacherName;
            CourseId = courseId;
            CourseName = courseName;
            CurrentCourseLoad = currentCourseLoad;
        }
    }
}
