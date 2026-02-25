using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when attendance is recorded for a student
    /// </summary>
    public sealed class AttendanceRecordedEvent : DomainEvent
    {
        /// <summary>
        /// Attendance record ID
        /// </summary>
        public Guid AttendanceId { get; }

        /// <summary>
        /// Student ID
        /// </summary>
        public Guid StudentId { get; }

        /// <summary>
        /// Course ID
        /// </summary>
        public Guid CourseId { get; }

        /// <summary>
        /// Attendance date
        /// </summary>
        public DateTime AttendanceDate { get; }

        /// <summary>
        /// Whether the student was present
        /// </summary>
        public bool IsPresent { get; }

        /// <summary>
        /// Initialize a new attendance recorded event
        /// </summary>
        /// <param name="attendanceId">Attendance record ID</param>
        /// <param name="studentId">Student ID</param>
        /// <param name="courseId">Course ID</param>
        /// <param name="attendanceDate">Attendance date</param>
        /// <param name="isPresent">Whether the student was present</param>
        public AttendanceRecordedEvent(
            Guid attendanceId, 
            Guid studentId, 
            Guid courseId, 
            DateTime attendanceDate, 
            bool isPresent)
        {
            AttendanceId = attendanceId;
            StudentId = studentId;
            CourseId = courseId;
            AttendanceDate = attendanceDate;
            IsPresent = isPresent;
        }
    }
}