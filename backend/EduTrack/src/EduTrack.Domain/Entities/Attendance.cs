using EduTrack.Domain.Common;
using EduTrack.Domain.Events;
using EduTrack.Domain.ValueObjects;

namespace EduTrack.Domain.Entities
{
    /// <summary>
    /// Attendance aggregate root representing a student's attendance record for a specific course session
    /// </summary>
    public class Attendance : AggregateRoot<Guid>
    {
        // Private backing fields for encapsulation
        private DateTime _attendanceDate;

        /// <summary>
        /// Student ID who the attendance record belongs to
        /// </summary>
        public Guid StudentId { get; private set; }

        /// <summary>
        /// Course ID for which attendance is being recorded
        /// </summary>
        public Guid CourseId { get; private set; }

        /// <summary>
        /// Date and time when attendance was taken
        /// </summary>
        public DateTime AttendanceDate 
        { 
            get => _attendanceDate;
            private set => _attendanceDate = ValidateAttendanceDate(value);
        }

        /// <summary>
        /// Whether the student was present (true) or absent (false)
        /// </summary>
        public bool IsPresent { get; private set; }

        /// <summary>
        /// Optional notes about the attendance (e.g., "Late arrival", "Excused absence")
        /// </summary>
        public string? Notes { get; private set; }

        /// <summary>
        /// Whether this attendance record has been verified by an instructor
        /// </summary>
        public bool IsVerified { get; private set; }

        /// <summary>
        /// When the attendance was initially recorded
        /// </summary>
        public DateTime RecordedAt { get; private set; }

        /// <summary>
        /// Who recorded the attendance (instructor, system, etc.)
        /// </summary>
        public string? RecordedBy { get; private set; }

        // Private constructor for EF Core
        private Attendance() : base()
        {
        }

        /// <summary>
        /// Record new attendance for a student in a course
        /// </summary>
        /// <param name="studentId">Student ID</param>
        /// <param name="courseId">Course ID</param>
        /// <param name="attendanceDate">Date of attendance</param>
        /// <param name="isPresent">Whether the student was present</param>
        /// <param name="recordedBy">Who recorded the attendance</param>
        /// <param name="notes">Optional notes</param>
        /// <returns>New attendance record</returns>
        public static Attendance RecordAttendance(
            Guid studentId,
            Guid courseId,
            DateTime attendanceDate,
            bool isPresent,
            string? recordedBy = null,
            string? notes = null)
        {
            if (studentId == Guid.Empty)
                throw new ArgumentException("Student ID cannot be empty", nameof(studentId));
            
            if (courseId == Guid.Empty)
                throw new ArgumentException("Course ID cannot be empty", nameof(courseId));

            var attendance = new Attendance
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                CourseId = courseId,
                _attendanceDate = ValidateAttendanceDate(attendanceDate),
                IsPresent = isPresent,
                Notes = ValidateNotes(notes),
                IsVerified = false,
                RecordedAt = DateTime.UtcNow,
                RecordedBy = recordedBy
            };

            // Raise domain event
            attendance.AddDomainEvent(new AttendanceRecordedEvent(
                attendance.Id, 
                studentId, 
                courseId, 
                attendanceDate, 
                isPresent));

            return attendance;
        }

        /// <summary>
        /// Mark the student as present
        /// </summary>
        /// <param name="notes">Optional notes</param>
        public void MarkPresent(string? notes = null)
        {
            if (IsPresent)
                return; // Already present, no change needed

            var previousStatus = IsPresent;
            IsPresent = true;
            Notes = ValidateNotes(notes);

            // Raise domain event
            AddDomainEvent(new AttendanceUpdatedEvent(
                Id, 
                StudentId, 
                CourseId, 
                AttendanceDate, 
                previousStatus, 
                IsPresent));
        }

        /// <summary>
        /// Mark the student as absent
        /// </summary>
        /// <param name="notes">Optional notes explaining the absence</param>
        public void MarkAbsent(string? notes = null)
        {
            if (!IsPresent)
                return; // Already absent, no change needed

            var previousStatus = IsPresent;
            IsPresent = false;
            Notes = ValidateNotes(notes);

            // Raise domain event
            AddDomainEvent(new AttendanceUpdatedEvent(
                Id, 
                StudentId, 
                CourseId, 
                AttendanceDate, 
                previousStatus, 
                IsPresent));
        }

        /// <summary>
        /// Verify the attendance record
        /// </summary>
        /// <param name="verifiedBy">Who verified the attendance</param>
        public void Verify(string verifiedBy)
        {
            if (string.IsNullOrWhiteSpace(verifiedBy))
                throw new ArgumentException("Verified by cannot be null or empty", nameof(verifiedBy));

            IsVerified = true;
            RecordedBy = verifiedBy;
        }

        /// <summary>
        /// Update attendance notes
        /// </summary>
        /// <param name="notes">New notes</param>
        public void UpdateNotes(string? notes)
        {
            Notes = ValidateNotes(notes);
        }

        /// <summary>
        /// Check if attendance can be modified (not older than a certain period)
        /// </summary>
        /// <param name="maxDaysForModification">Maximum days after which attendance cannot be modified</param>
        /// <returns>True if attendance can be modified</returns>
        public bool CanBeModified(int maxDaysForModification = 7)
        {
            return DateTime.UtcNow.Date.Subtract(AttendanceDate.Date).TotalDays <= maxDaysForModification;
        }

        /// <summary>
        /// Check if the attendance is for today
        /// </summary>
        /// <returns>True if attendance is for today</returns>
        public bool IsForToday()
        {
            return AttendanceDate.Date == DateTime.Today;
        }

        // Private validation methods
        private static DateTime ValidateAttendanceDate(DateTime date)
        {
            if (date > DateTime.Now)
                throw new ArgumentException("Attendance date cannot be in the future", nameof(date));

            if (date < new DateTime(1900, 1, 1))
                throw new ArgumentException("Attendance date is not valid", nameof(date));

            return date;
        }

        private static string? ValidateNotes(string? notes)
        {
            if (notes != null && notes.Length > 500)
                throw new ArgumentException("Notes cannot exceed 500 characters", nameof(notes));

            return string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        }
    }
}
