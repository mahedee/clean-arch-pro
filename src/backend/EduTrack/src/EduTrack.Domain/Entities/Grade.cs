using EduTrack.Domain.Common;
using EduTrack.Domain.Enums;
using EduTrack.Domain.Events;

namespace EduTrack.Domain.Entities
{
    /// <summary>
    /// Grade aggregate root representing a student's grade record for a specific assessment or enrollment
    /// </summary>
    public class Grade : AggregateRoot<Guid>
    {
        /// <summary>
        /// ID of the enrollment this grade belongs to
        /// </summary>
        public Guid EnrollmentId { get; private set; }

        /// <summary>
        /// ID of the student who received this grade
        /// </summary>
        public Guid StudentId { get; private set; }

        /// <summary>
        /// ID of the course for which this grade was given
        /// </summary>
        public Guid CourseId { get; private set; }

        /// <summary>
        /// ID of the associated assessment (optional – null for final course grade)
        /// </summary>
        public Guid? AssessmentId { get; private set; }

        /// <summary>
        /// Score achieved by the student
        /// </summary>
        public decimal Score { get; private set; }

        /// <summary>
        /// Maximum possible score for this assessment
        /// </summary>
        public decimal MaxScore { get; private set; }

        /// <summary>
        /// Calculated percentage score (Score / MaxScore * 100)
        /// </summary>
        public decimal Percentage => MaxScore > 0 ? Math.Round(Score / MaxScore * 100, 2) : 0;

        /// <summary>
        /// Letter grade assigned (e.g., "A", "B+", "C")
        /// </summary>
        public string? LetterGrade { get; private set; }

        /// <summary>
        /// Grade points used for GPA calculation (0.0 – 4.0)
        /// </summary>
        public decimal GradePoints { get; private set; }

        /// <summary>
        /// Optional notes about the grade (e.g., reason for deductions)
        /// </summary>
        public string? Notes { get; private set; }

        /// <summary>
        /// Date and time the grade was recorded
        /// </summary>
        public DateTime GradedAt { get; private set; }

        /// <summary>
        /// Name of the person who assigned the grade
        /// </summary>
        public string? GradedBy { get; private set; }

        /// <summary>
        /// Current lifecycle status of this grade
        /// </summary>
        public GradeStatus Status { get; private set; }

        // Private constructor for EF Core
        private Grade() : base() { }

        /// <summary>
        /// Record a grade for a student's assessment or enrollment
        /// </summary>
        /// <param name="enrollmentId">ID of the related enrollment</param>
        /// <param name="studentId">ID of the student</param>
        /// <param name="courseId">ID of the course</param>
        /// <param name="score">Score achieved</param>
        /// <param name="maxScore">Maximum possible score</param>
        /// <param name="gradedBy">Person recording the grade</param>
        /// <param name="assessmentId">Optional associated assessment ID</param>
        /// <param name="notes">Optional grading notes</param>
        /// <returns>New grade instance</returns>
        public static Grade Record(
            Guid enrollmentId,
            Guid studentId,
            Guid courseId,
            decimal score,
            decimal maxScore,
            string? gradedBy = null,
            Guid? assessmentId = null,
            string? notes = null)
        {
            if (enrollmentId == Guid.Empty)
                throw new ArgumentException("Enrollment ID cannot be empty.", nameof(enrollmentId));
            if (studentId == Guid.Empty)
                throw new ArgumentException("Student ID cannot be empty.", nameof(studentId));
            if (courseId == Guid.Empty)
                throw new ArgumentException("Course ID cannot be empty.", nameof(courseId));
            if (maxScore <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxScore), "Maximum score must be greater than zero.");
            if (score < 0 || score > maxScore)
                throw new ArgumentOutOfRangeException(nameof(score), "Score must be between 0 and the maximum score.");

            var percentage = score / maxScore * 100;
            var letterGrade = CalculateLetterGrade(percentage);
            var gradePoints = CalculateGradePoints(percentage);

            return new Grade
            {
                Id = Guid.NewGuid(),
                EnrollmentId = enrollmentId,
                StudentId = studentId,
                CourseId = courseId,
                AssessmentId = assessmentId,
                Score = score,
                MaxScore = maxScore,
                LetterGrade = letterGrade,
                GradePoints = gradePoints,
                Notes = notes,
                GradedAt = DateTime.UtcNow,
                GradedBy = gradedBy,
                Status = GradeStatus.Pending
            };
        }

        /// <summary>
        /// Submit the grade for approval
        /// </summary>
        public void Submit()
        {
            if (Status != GradeStatus.Pending)
                throw new InvalidOperationException("Only pending grades can be submitted.");

            Status = GradeStatus.Submitted;
            MarkAsUpdated();
        }

        /// <summary>
        /// Approve the submitted grade
        /// </summary>
        /// <param name="approvedBy">Name of the approver</param>
        public void Approve(string approvedBy)
        {
            if (Status != GradeStatus.Submitted)
                throw new InvalidOperationException("Only submitted grades can be approved.");

            Status = GradeStatus.Approved;
            MarkAsUpdated(approvedBy);
        }

        /// <summary>
        /// Publish the grade so the student can view it
        /// </summary>
        public void Publish()
        {
            if (Status != GradeStatus.Approved)
                throw new InvalidOperationException("Only approved grades can be published.");

            Status = GradeStatus.Published;
            MarkAsUpdated();

            AddDomainEvent(new GradePublishedEvent(Id, StudentId, CourseId, Score, LetterGrade));
        }

        /// <summary>
        /// Mark the grade as disputed by the student
        /// </summary>
        public void Dispute()
        {
            if (Status != GradeStatus.Published)
                throw new InvalidOperationException("Only published grades can be disputed.");

            Status = GradeStatus.Disputed;
            MarkAsUpdated();
        }

        /// <summary>
        /// Revise the grade (after a dispute or correction)
        /// </summary>
        /// <param name="newScore">Revised score</param>
        /// <param name="revisedBy">Person making the revision</param>
        /// <param name="notes">Reason for revision</param>
        public void Revise(decimal newScore, string revisedBy, string? notes = null)
        {
            if (Status != GradeStatus.Disputed && Status != GradeStatus.Published)
                throw new InvalidOperationException("Only disputed or published grades can be revised.");
            if (newScore < 0 || newScore > MaxScore)
                throw new ArgumentOutOfRangeException(nameof(newScore), "Revised score must be between 0 and the maximum score.");

            var percentage = newScore / MaxScore * 100;
            Score = newScore;
            LetterGrade = CalculateLetterGrade(percentage);
            GradePoints = CalculateGradePoints(percentage);
            Notes = notes;
            Status = GradeStatus.Revised;
            MarkAsUpdated(revisedBy);
        }

        // Letter grade calculation based on standard percentage thresholds
        private static string CalculateLetterGrade(decimal percentage)
        {
            return percentage switch
            {
                >= 93 => "A",
                >= 90 => "A-",
                >= 87 => "B+",
                >= 83 => "B",
                >= 80 => "B-",
                >= 77 => "C+",
                >= 73 => "C",
                >= 70 => "C-",
                >= 67 => "D+",
                >= 63 => "D",
                >= 60 => "D-",
                _ => "F"
            };
        }

        // GPA points calculation based on letter grade
        private static decimal CalculateGradePoints(decimal percentage)
        {
            return percentage switch
            {
                >= 93 => 4.0m,
                >= 90 => 3.7m,
                >= 87 => 3.3m,
                >= 83 => 3.0m,
                >= 80 => 2.7m,
                >= 77 => 2.3m,
                >= 73 => 2.0m,
                >= 70 => 1.7m,
                >= 67 => 1.3m,
                >= 63 => 1.0m,
                >= 60 => 0.7m,
                _ => 0.0m
            };
        }
    }
}
