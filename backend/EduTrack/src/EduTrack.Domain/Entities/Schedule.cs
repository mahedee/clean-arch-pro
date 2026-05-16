using EduTrack.Domain.Common;
using EduTrack.Domain.Enums;

namespace EduTrack.Domain.Entities
{
    /// <summary>
    /// Schedule aggregate root representing a class schedule entry linking a course,
    /// teacher, room, and time slot for a given academic semester
    /// </summary>
    public class Schedule : AggregateRoot<Guid>
    {
        // Private backing fields for encapsulation
        private string _room = null!;
        private TimeSpan _startTime;
        private TimeSpan _endTime;

        /// <summary>
        /// ID of the course being scheduled
        /// </summary>
        public Guid CourseId { get; private set; }

        /// <summary>
        /// ID of the teacher assigned to this schedule
        /// </summary>
        public Guid TeacherId { get; private set; }

        /// <summary>
        /// Day of the week this class is held
        /// </summary>
        public ClassDay DayOfWeek { get; private set; }

        /// <summary>
        /// Start time of the class session
        /// </summary>
        public TimeSpan StartTime
        {
            get => _startTime;
            private set
            {
                if (value < TimeSpan.Zero || value >= TimeSpan.FromHours(24))
                    throw new ArgumentOutOfRangeException(nameof(StartTime), "Start time must be a valid time of day.");
                _startTime = value;
            }
        }

        /// <summary>
        /// End time of the class session
        /// </summary>
        public TimeSpan EndTime
        {
            get => _endTime;
            private set
            {
                if (value < TimeSpan.Zero || value >= TimeSpan.FromHours(24))
                    throw new ArgumentOutOfRangeException(nameof(EndTime), "End time must be a valid time of day.");
                _endTime = value;
            }
        }

        /// <summary>
        /// Room or classroom where the class is held
        /// </summary>
        public string Room
        {
            get => _room;
            private set => _room = ValidateRoom(value);
        }

        /// <summary>
        /// Building where the classroom is located (optional)
        /// </summary>
        public string? Building { get; private set; }

        /// <summary>
        /// Academic semester for this schedule (e.g., "Fall 2026")
        /// </summary>
        public string Semester { get; private set; } = null!;

        /// <summary>
        /// Academic year for this schedule
        /// </summary>
        public int AcademicYear { get; private set; }

        /// <summary>
        /// Current status of the schedule
        /// </summary>
        public ScheduleStatus Status { get; private set; }

        /// <summary>
        /// Optional notes about this schedule entry
        /// </summary>
        public string? Notes { get; private set; }

        // Private constructor for EF Core
        private Schedule() : base() { }

        /// <summary>
        /// Create a new class schedule
        /// </summary>
        /// <param name="courseId">ID of the course</param>
        /// <param name="teacherId">ID of the assigned teacher</param>
        /// <param name="dayOfWeek">Day of the week the class is held</param>
        /// <param name="startTime">Class start time</param>
        /// <param name="endTime">Class end time</param>
        /// <param name="room">Room or classroom identifier</param>
        /// <param name="semester">Academic semester</param>
        /// <param name="academicYear">Academic year</param>
        /// <param name="building">Optional building name</param>
        /// <returns>New schedule instance</returns>
        public static Schedule Create(
            Guid courseId,
            Guid teacherId,
            ClassDay dayOfWeek,
            TimeSpan startTime,
            TimeSpan endTime,
            string room,
            string semester,
            int academicYear,
            string? building = null)
        {
            if (courseId == Guid.Empty)
                throw new ArgumentException("Course ID cannot be empty.", nameof(courseId));
            if (teacherId == Guid.Empty)
                throw new ArgumentException("Teacher ID cannot be empty.", nameof(teacherId));
            if (string.IsNullOrWhiteSpace(semester))
                throw new ArgumentException("Semester cannot be empty.", nameof(semester));
            if (academicYear < 2000 || academicYear > DateTime.UtcNow.Year + 2)
                throw new ArgumentOutOfRangeException(nameof(academicYear), "Academic year is not valid.");
            if (endTime <= startTime)
                throw new ArgumentException("End time must be after start time.", nameof(endTime));

            return new Schedule
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                TeacherId = teacherId,
                DayOfWeek = dayOfWeek,
                StartTime = startTime,
                EndTime = endTime,
                Room = room,
                Building = building,
                Semester = semester.Trim(),
                AcademicYear = academicYear,
                Status = ScheduleStatus.Scheduled
            };
        }

        /// <summary>
        /// Activate the schedule (class sessions begin)
        /// </summary>
        public void Activate()
        {
            if (Status != ScheduleStatus.Scheduled && Status != ScheduleStatus.Postponed)
                throw new InvalidOperationException("Only scheduled or postponed schedules can be activated.");

            Status = ScheduleStatus.Active;
            MarkAsUpdated();
        }

        /// <summary>
        /// Postpone the schedule to a later date
        /// </summary>
        /// <param name="notes">Optional reason or details about the postponement</param>
        public void Postpone(string? notes = null)
        {
            if (Status != ScheduleStatus.Scheduled && Status != ScheduleStatus.Active)
                throw new InvalidOperationException("Only scheduled or active schedules can be postponed.");

            Status = ScheduleStatus.Postponed;
            Notes = notes;
            MarkAsUpdated();
        }

        /// <summary>
        /// Cancel the schedule
        /// </summary>
        /// <param name="notes">Optional reason for cancellation</param>
        public void Cancel(string? notes = null)
        {
            if (Status == ScheduleStatus.Completed || Status == ScheduleStatus.Cancelled)
                throw new InvalidOperationException("Completed or already cancelled schedules cannot be cancelled.");

            Status = ScheduleStatus.Cancelled;
            Notes = notes;
            MarkAsUpdated();
        }

        /// <summary>
        /// Mark the schedule as completed (end of semester)
        /// </summary>
        public void Complete()
        {
            if (Status != ScheduleStatus.Active)
                throw new InvalidOperationException("Only active schedules can be completed.");

            Status = ScheduleStatus.Completed;
            MarkAsUpdated();
        }

        /// <summary>
        /// Reassign the schedule to a different teacher
        /// </summary>
        /// <param name="newTeacherId">ID of the new teacher</param>
        public void ReassignTeacher(Guid newTeacherId)
        {
            if (newTeacherId == Guid.Empty)
                throw new ArgumentException("New teacher ID cannot be empty.", nameof(newTeacherId));
            if (Status == ScheduleStatus.Completed || Status == ScheduleStatus.Cancelled)
                throw new InvalidOperationException("Completed or cancelled schedules cannot be reassigned.");

            TeacherId = newTeacherId;
            MarkAsUpdated();
        }

        /// <summary>
        /// Update the room assignment
        /// </summary>
        /// <param name="room">New room identifier</param>
        /// <param name="building">Optional new building name</param>
        public void UpdateRoom(string room, string? building = null)
        {
            if (Status == ScheduleStatus.Completed || Status == ScheduleStatus.Cancelled)
                throw new InvalidOperationException("Completed or cancelled schedules cannot be updated.");

            Room = room;
            Building = building;
            MarkAsUpdated();
        }

        /// <summary>
        /// Update the class time slot
        /// </summary>
        /// <param name="startTime">New start time</param>
        /// <param name="endTime">New end time</param>
        public void UpdateTimeSlot(TimeSpan startTime, TimeSpan endTime)
        {
            if (endTime <= startTime)
                throw new ArgumentException("End time must be after start time.", nameof(endTime));
            if (Status == ScheduleStatus.Completed || Status == ScheduleStatus.Cancelled)
                throw new InvalidOperationException("Completed or cancelled schedules cannot be updated.");

            StartTime = startTime;
            EndTime = endTime;
            MarkAsUpdated();
        }

        // Private validation helpers
        private static string ValidateRoom(string room)
        {
            if (string.IsNullOrWhiteSpace(room))
                throw new ArgumentException("Room cannot be empty.");
            if (room.Length > 50)
                throw new ArgumentException("Room identifier cannot exceed 50 characters.");
            return room.Trim();
        }
    }
}
