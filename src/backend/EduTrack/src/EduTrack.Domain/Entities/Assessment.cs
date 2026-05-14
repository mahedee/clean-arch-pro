using EduTrack.Domain.Common;
using EduTrack.Domain.Enums;
using EduTrack.Domain.Events;

namespace EduTrack.Domain.Entities
{
    /// <summary>
    /// Assessment aggregate root representing an academic assessment (quiz, assignment, exam, etc.)
    /// defined for a course
    /// </summary>
    public class Assessment : AggregateRoot<Guid>
    {
        // Private backing fields for encapsulation
        private string _title = null!;
        private string _description = null!;
        private decimal _maxScore;
        private decimal _weightPercentage;

        /// <summary>
        /// ID of the course this assessment belongs to
        /// </summary>
        public Guid CourseId { get; private set; }

        /// <summary>
        /// Title of the assessment (e.g., "Midterm Exam", "Assignment 1")
        /// </summary>
        public string Title
        {
            get => _title;
            private set => _title = ValidateTitle(value);
        }

        /// <summary>
        /// Description or instructions for the assessment
        /// </summary>
        public string Description
        {
            get => _description;
            private set => _description = value?.Trim() ?? string.Empty;
        }

        /// <summary>
        /// Category of the assessment
        /// </summary>
        public AssessmentType Type { get; private set; }

        /// <summary>
        /// Maximum score achievable on this assessment
        /// </summary>
        public decimal MaxScore
        {
            get => _maxScore;
            private set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(MaxScore), "Maximum score must be greater than zero.");
                _maxScore = value;
            }
        }

        /// <summary>
        /// Percentage weight of this assessment towards the final course grade (0.0 – 100.0)
        /// </summary>
        public decimal WeightPercentage
        {
            get => _weightPercentage;
            private set
            {
                if (value < 0 || value > 100)
                    throw new ArgumentOutOfRangeException(nameof(WeightPercentage), "Weight percentage must be between 0 and 100.");
                _weightPercentage = value;
            }
        }

        /// <summary>
        /// Date and time the assessment is due
        /// </summary>
        public DateTime DueDate { get; private set; }

        /// <summary>
        /// Date and time the assessment becomes available to students (optional)
        /// </summary>
        public DateTime? AvailableFrom { get; private set; }

        /// <summary>
        /// Whether the assessment allows late submissions
        /// </summary>
        public bool AllowLateSubmission { get; private set; }

        /// <summary>
        /// Score deducted per day for late submissions (0 = no penalty)
        /// </summary>
        public decimal LateSubmissionPenaltyPerDay { get; private set; }

        /// <summary>
        /// Current lifecycle status of the assessment
        /// </summary>
        public AssessmentStatus Status { get; private set; }

        // Private constructor for EF Core
        private Assessment() : base() { }

        /// <summary>
        /// Create a new assessment for a course
        /// </summary>
        /// <param name="courseId">ID of the course</param>
        /// <param name="title">Assessment title</param>
        /// <param name="description">Assessment instructions/description</param>
        /// <param name="type">Assessment category</param>
        /// <param name="maxScore">Maximum achievable score</param>
        /// <param name="weightPercentage">Weight towards final grade</param>
        /// <param name="dueDate">Submission deadline</param>
        /// <param name="availableFrom">When the assessment becomes available (optional)</param>
        /// <returns>New assessment instance in Draft status</returns>
        public static Assessment Create(
            Guid courseId,
            string title,
            string description,
            AssessmentType type,
            decimal maxScore,
            decimal weightPercentage,
            DateTime dueDate,
            DateTime? availableFrom = null)
        {
            if (courseId == Guid.Empty)
                throw new ArgumentException("Course ID cannot be empty.", nameof(courseId));
            if (dueDate <= DateTime.UtcNow)
                throw new ArgumentException("Due date must be in the future.", nameof(dueDate));
            if (availableFrom.HasValue && availableFrom.Value >= dueDate)
                throw new ArgumentException("Available-from date must be before the due date.", nameof(availableFrom));

            return new Assessment
            {
                Id = Guid.NewGuid(),
                CourseId = courseId,
                Title = title,
                Description = description,
                Type = type,
                MaxScore = maxScore,
                WeightPercentage = weightPercentage,
                DueDate = dueDate,
                AvailableFrom = availableFrom,
                AllowLateSubmission = false,
                LateSubmissionPenaltyPerDay = 0,
                Status = AssessmentStatus.Draft
            };
        }

        /// <summary>
        /// Publish the assessment so students can view and submit it
        /// </summary>
        public void Publish()
        {
            if (Status != AssessmentStatus.Draft)
                throw new InvalidOperationException("Only draft assessments can be published.");

            Status = AssessmentStatus.Published;
            MarkAsUpdated();

            AddDomainEvent(new AssessmentPublishedEvent(Id, CourseId, Title, DueDate));
        }

        /// <summary>
        /// Activate the assessment (open for submissions)
        /// </summary>
        public void Activate()
        {
            if (Status != AssessmentStatus.Published)
                throw new InvalidOperationException("Only published assessments can be activated.");

            Status = AssessmentStatus.Active;
            MarkAsUpdated();
        }

        /// <summary>
        /// Close the assessment (no more submissions accepted)
        /// </summary>
        public void Close()
        {
            if (Status != AssessmentStatus.Active)
                throw new InvalidOperationException("Only active assessments can be closed.");

            Status = AssessmentStatus.Closed;
            MarkAsUpdated();
        }

        /// <summary>
        /// Mark the assessment as fully graded
        /// </summary>
        public void MarkAsGraded()
        {
            if (Status != AssessmentStatus.Closed)
                throw new InvalidOperationException("Only closed assessments can be marked as graded.");

            Status = AssessmentStatus.Graded;
            MarkAsUpdated();
        }

        /// <summary>
        /// Configure late submission policy
        /// </summary>
        /// <param name="allow">Whether late submissions are allowed</param>
        /// <param name="penaltyPerDay">Score deducted per day late (0 for no penalty)</param>
        public void SetLateSubmissionPolicy(bool allow, decimal penaltyPerDay = 0)
        {
            if (penaltyPerDay < 0 || penaltyPerDay > MaxScore)
                throw new ArgumentOutOfRangeException(nameof(penaltyPerDay), "Penalty per day cannot be negative or exceed the maximum score.");
            if (Status == AssessmentStatus.Graded)
                throw new InvalidOperationException("Cannot update policy for graded assessments.");

            AllowLateSubmission = allow;
            LateSubmissionPenaltyPerDay = allow ? penaltyPerDay : 0;
            MarkAsUpdated();
        }

        /// <summary>
        /// Update the assessment details (only allowed in Draft status)
        /// </summary>
        /// <param name="title">New title</param>
        /// <param name="description">New description</param>
        /// <param name="maxScore">New maximum score</param>
        /// <param name="weightPercentage">New weight percentage</param>
        /// <param name="dueDate">New due date</param>
        public void UpdateDetails(string title, string description, decimal maxScore, decimal weightPercentage, DateTime dueDate)
        {
            if (Status != AssessmentStatus.Draft)
                throw new InvalidOperationException("Only draft assessments can be updated.");
            if (dueDate <= DateTime.UtcNow)
                throw new ArgumentException("Due date must be in the future.", nameof(dueDate));

            Title = title;
            Description = description;
            MaxScore = maxScore;
            WeightPercentage = weightPercentage;
            DueDate = dueDate;
            MarkAsUpdated();
        }

        // Private validation helpers
        private static string ValidateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Assessment title cannot be empty.");
            if (title.Length > 200)
                throw new ArgumentException("Assessment title cannot exceed 200 characters.");
            return title.Trim();
        }
    }
}
