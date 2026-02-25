using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when attendance record is updated
    /// </summary>
    public sealed class AttendanceUpdatedEvent : DomainEvent
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
        /// Previous attendance status
        /// </summary>
        public bool PreviousStatus { get; }

        /// <summary>
        /// New attendance status
        /// </summary>
        public bool NewStatus { get; }

        /// <summary>
        /// Initialize a new attendance updated event
        /// </summary>
        /// <param name="attendanceId">Attendance record ID</param>
        /// <param name="studentId">Student ID</param>
        /// <param name="courseId">Course ID</param>
        /// <param name="attendanceDate">Attendance date</param>
        /// <param name="previousStatus">Previous attendance status</param>
        /// <param name="newStatus">New attendance status</param>
        public AttendanceUpdatedEvent(
            Guid attendanceId, 
            Guid studentId, 
            Guid courseId, 
            DateTime attendanceDate, 
            bool previousStatus, 
            bool newStatus)
        {
            AttendanceId = attendanceId;
            StudentId = studentId;
            CourseId = courseId;
            AttendanceDate = attendanceDate;
            PreviousStatus = previousStatus;
            NewStatus = newStatus;
        }
    }
}