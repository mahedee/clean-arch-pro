using EduTrack.Domain.Common;
using EduTrack.Domain.Enums;
using EduTrack.Domain.Events;

namespace EduTrack.Domain.Entities
{
    /// <summary>
    /// Enrollment aggregate root representing a student's registration in a course for a given semester
    /// </summary>
    public class Enrollment : AggregateRoot<Guid>
    {
        /// <summary>
        /// ID of the enrolled student
        /// </summary>
        public Guid StudentId { get; private set; }

        /// <summary>
        /// ID of the course the student is enrolled in
        /// </summary>
        public Guid CourseId { get; private set; }

        /// <summary>
        /// Date and time the enrollment was created
        /// </summary>
        public DateTime EnrollmentDate { get; private set; }

        /// <summary>
        /// Current enrollment status
        /// </summary>
        public EnrollmentStatus Status { get; private set; }

        /// <summary>
        /// Final grade achieved upon course completion (0.0 – 100.0)
        /// </summary>
        public decimal? FinalGrade { get; private set; }

        /// <summary>
        /// Date the enrollment was completed
        /// </summary>
        public DateTime? CompletionDate { get; private set; }

        /// <summary>
        /// Date the student withdrew or dropped
        /// </summary>
        public DateTime? WithdrawalDate { get; private set; }

        /// <summary>
        /// Reason provided when the student withdrew or dropped
        /// </summary>
        public string? WithdrawalReason { get; private set; }

        /// <summary>
        /// Academic semester for this enrollment (e.g., "Fall", "Spring")
        /// </summary>
        public string Semester { get; private set; } = null!;

        /// <summary>
        /// Academic year for this enrollment (e.g., 2026)
        /// </summary>
        public int AcademicYear { get; private set; }

        // Private constructor for EF Core
        private Enrollment() : base() { }

        /// <summary>
        /// Enroll a student in a course
        /// </summary>
        /// <param name="studentId">ID of the student</param>
        /// <param name="courseId">ID of the course</param>
        /// <param name="semester">Academic semester (e.g., "Fall 2026")</param>
        /// <param name="academicYear">Academic year</param>
        /// <returns>New enrollment instance</returns>
        public static Enrollment Enroll(Guid studentId, Guid courseId, string semester, int academicYear)
        {
            if (studentId == Guid.Empty)
                throw new ArgumentException("Student ID cannot be empty.", nameof(studentId));
            if (courseId == Guid.Empty)
                throw new ArgumentException("Course ID cannot be empty.", nameof(courseId));
            if (string.IsNullOrWhiteSpace(semester))
                throw new ArgumentException("Semester cannot be empty.", nameof(semester));
            if (academicYear < 2000 || academicYear > DateTime.UtcNow.Year + 2)
                throw new ArgumentOutOfRangeException(nameof(academicYear), "Academic year is not valid.");

            var enrollment = new Enrollment
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                CourseId = courseId,
                EnrollmentDate = DateTime.UtcNow,
                Status = EnrollmentStatus.Active,
                Semester = semester.Trim(),
                AcademicYear = academicYear
            };

            enrollment.AddDomainEvent(new EnrollmentCreatedEvent(enrollment.Id, studentId, courseId, semester, academicYear));
            return enrollment;
        }

        /// <summary>
        /// Complete the enrollment and record the final grade
        /// </summary>
        /// <param name="finalGrade">Final grade between 0.0 and 100.0</param>
        public void Complete(decimal finalGrade)
        {
            if (Status != EnrollmentStatus.Active && Status != EnrollmentStatus.OnHold)
                throw new InvalidOperationException("Only active or on-hold enrollments can be completed.");
            if (finalGrade < 0 || finalGrade > 100)
                throw new ArgumentOutOfRangeException(nameof(finalGrade), "Final grade must be between 0 and 100.");

            Status = EnrollmentStatus.Completed;
            FinalGrade = finalGrade;
            CompletionDate = DateTime.UtcNow;
            MarkAsUpdated();

            AddDomainEvent(new EnrollmentCompletedEvent(Id, StudentId, CourseId, finalGrade));
        }

        /// <summary>
        /// Withdraw from the enrollment
        /// </summary>
        /// <param name="reason">Optional reason for withdrawal</param>
        public void Withdraw(string? reason = null)
        {
            if (Status == EnrollmentStatus.Completed)
                throw new InvalidOperationException("Cannot withdraw from a completed enrollment.");
            if (Status == EnrollmentStatus.Withdrawn)
                throw new InvalidOperationException("Enrollment is already withdrawn.");

            Status = EnrollmentStatus.Withdrawn;
            WithdrawalDate = DateTime.UtcNow;
            WithdrawalReason = reason;
            MarkAsUpdated();

            AddDomainEvent(new EnrollmentWithdrawnEvent(Id, StudentId, CourseId, reason));
        }

        /// <summary>
        /// Drop the enrollment (early-term drop, before withdrawal period)
        /// </summary>
        /// <param name="reason">Optional reason for dropping</param>
        public void Drop(string? reason = null)
        {
            if (Status != EnrollmentStatus.Active)
                throw new InvalidOperationException("Only active enrollments can be dropped.");

            Status = EnrollmentStatus.Dropped;
            WithdrawalDate = DateTime.UtcNow;
            WithdrawalReason = reason;
            MarkAsUpdated();
        }

        /// <summary>
        /// Put the enrollment on hold (e.g., medical leave)
        /// </summary>
        public void PutOnHold()
        {
            if (Status != EnrollmentStatus.Active)
                throw new InvalidOperationException("Only active enrollments can be put on hold.");

            Status = EnrollmentStatus.OnHold;
            MarkAsUpdated();
        }

        /// <summary>
        /// Reactivate a previously held enrollment
        /// </summary>
        public void Reactivate()
        {
            if (Status != EnrollmentStatus.OnHold)
                throw new InvalidOperationException("Only on-hold enrollments can be reactivated.");

            Status = EnrollmentStatus.Active;
            MarkAsUpdated();
        }
    }
}
