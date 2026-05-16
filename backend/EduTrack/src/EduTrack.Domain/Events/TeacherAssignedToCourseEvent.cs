using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a teacher is assigned to a course
    /// </summary>
    public class TeacherAssignedToCourseEvent : DomainEvent
    {
        public Guid TeacherId { get; }
        public string TeacherName { get; }
        public Guid CourseId { get; }
        public string CourseName { get; }
        public int CurrentCourseLoad { get; }

        public TeacherAssignedToCourseEvent(Guid teacherId, string teacherName, Guid courseId, string courseName, int currentCourseLoad)
        {
            TeacherId = teacherId;
            TeacherName = teacherName;
            CourseId = courseId;
            CourseName = courseName;
            CurrentCourseLoad = currentCourseLoad;
        }
    }
}
