using EduTrack.Domain.Common;
using EduTrack.Domain.Events;

namespace EduTrack.Domain.Entities
{
    /// <summary>
    /// Course aggregate root representing a course in the education system
    /// </summary>
    public class Course : AggregateRoot<Guid>
    {
        // Private backing fields for encapsulation
        private string _title = null!;
        private string _code = null!;
        private string _description = null!;

        /// <summary>
        /// Course title
        /// </summary>
        public string Title 
        { 
            get => _title;
            private set => _title = ValidateTitle(value);
        }

        /// <summary>
        /// Course code (e.g., "CS101", "MATH201")
        /// </summary>
        public string Code 
        { 
            get => _code;
            private set => _code = ValidateCode(value);
        }

        /// <summary>
        /// Course description
        /// </summary>
        public string Description 
        { 
            get => _description;
            private set => _description = ValidateDescription(value);
        }

        /// <summary>
        /// Number of credit hours for the course
        /// </summary>
        public int CreditHours { get; private set; }

        /// <summary>
        /// Minimum credit hours required as prerequisites
        /// </summary>
        public int PrerequisiteCreditHours { get; private set; }

        /// <summary>
        /// Course level (Undergraduate, Graduate, etc.)
        /// </summary>
        public CourseLevel Level { get; private set; }

        /// <summary>
        /// Course status (Active, Inactive, etc.)
        /// </summary>
        public CourseStatus Status { get; private set; }

        /// <summary>
        /// Maximum number of students that can enroll
        /// </summary>
        public int MaxEnrollment { get; private set; }

        /// <summary>
        /// Current number of enrolled students
        /// </summary>
        public int CurrentEnrollment { get; private set; }

        /// <summary>
        /// Department or subject area
        /// </summary>
        public string Department { get; private set; } = null!;

        /// <summary>
        /// Academic semester/term when course is offered
        /// </summary>
        public string Semester { get; private set; } = null!;

        /// <summary>
        /// Academic year when course is offered
        /// </summary>
        public int AcademicYear { get; private set; }

        /// <summary>
        /// Course start date
        /// </summary>
        public DateTime StartDate { get; private set; }

        /// <summary>
        /// Course end date
        /// </summary>
        public DateTime EndDate { get; private set; }

        // Private constructor for EF Core
        private Course() : base()
        {
        }

        /// <summary>
        /// Create a new course
        /// </summary>
        /// <param name="title">Course title</param>
        /// <param name="code">Course code</param>
        /// <param name="description">Course description</param>
        /// <param name="creditHours">Number of credit hours</param>
        /// <param name="level">Course level</param>
        /// <param name="department">Department</param>
        /// <param name="maxEnrollment">Maximum enrollment</param>
        /// <returns>New course instance</returns>
        public static Course Create(
            string title, 
            string code, 
            string description, 
            int creditHours, 
            CourseLevel level, 
            string department,
            int maxEnrollment = 30)
        {
            var course = new Course
            {
                Id = Guid.NewGuid(),
                _title = ValidateTitle(title),
                _code = ValidateCode(code),
                _description = ValidateDescription(description),
                CreditHours = ValidateCreditHours(creditHours),
                Level = level,
                Department = ValidateDepartment(department),
                MaxEnrollment = ValidateMaxEnrollment(maxEnrollment),
                CurrentEnrollment = 0,
                PrerequisiteCreditHours = 0,
                Status = CourseStatus.Draft,
                Semester = "",
                AcademicYear = DateTime.Now.Year
            };

            // Raise domain event
            course.AddDomainEvent(new CourseCreatedEvent(course.Id, course.Title, course.Code, course.Department));

            return course;
        }

        /// <summary>
        /// Schedule the course for a specific semester and academic year
        /// </summary>
        /// <param name="semester">Semester (e.g., "Fall", "Spring", "Summer")</param>
        /// <param name="academicYear">Academic year</param>
        /// <param name="startDate">Course start date</param>
        /// <param name="endDate">Course end date</param>
        public void Schedule(string semester, int academicYear, DateTime startDate, DateTime endDate)
        {
            if (string.IsNullOrWhiteSpace(semester))
                throw new ArgumentException("Semester cannot be empty", nameof(semester));

            if (academicYear < 2000 || academicYear > 2100)
                throw new ArgumentException("Academic year must be between 2000 and 2100", nameof(academicYear));

            if (startDate >= endDate)
                throw new ArgumentException("Start date must be before end date");

            if (startDate < DateTime.Today)
                throw new ArgumentException("Start date cannot be in the past");

            Semester = semester;
            AcademicYear = academicYear;
            StartDate = startDate;
            EndDate = endDate;
            Status = CourseStatus.Scheduled;
            
            MarkAsUpdated();

            // Raise domain event
            AddDomainEvent(new CourseScheduledEvent(Id, semester, academicYear, startDate, endDate));
        }

        /// <summary>
        /// Activate the course (make it available for enrollment)
        /// </summary>
        public void Activate()
        {
            if (Status == CourseStatus.Draft)
                throw new InvalidOperationException("Cannot activate a course that hasn't been scheduled");

            Status = CourseStatus.Active;
            MarkAsUpdated();

            // Raise domain event
            AddDomainEvent(new CourseActivatedEvent(Id, Title, Code));
        }

        /// <summary>
        /// Deactivate the course
        /// </summary>
        public void Deactivate()
        {
            Status = CourseStatus.Inactive;
            MarkAsUpdated();
        }

        /// <summary>
        /// Cancel the course
        /// </summary>
        /// <param name="reason">Reason for cancellation</param>
        public void Cancel(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Cancellation reason is required", nameof(reason));

            Status = CourseStatus.Cancelled;
            MarkAsUpdated();

            // Raise domain event
            AddDomainEvent(new CourseCancelledEvent(Id, Title, Code, reason));
        }

        /// <summary>
        /// Complete the course
        /// </summary>
        public void Complete()
        {
            if (Status != CourseStatus.Active)
                throw new InvalidOperationException("Only active courses can be completed");

            // Allow manual completion regardless of end date for administrative purposes
            Status = CourseStatus.Completed;
            MarkAsUpdated();

            // Raise domain event
            AddDomainEvent(new CourseCompletedEvent(Id, Title, Code, CurrentEnrollment));
        }

        /// <summary>
        /// Update course information
        /// </summary>
        /// <param name="title">New title</param>
        /// <param name="description">New description</param>
        /// <param name="maxEnrollment">New maximum enrollment</param>
        public void UpdateCourseInfo(string title, string description, int maxEnrollment)
        {
            if (Status == CourseStatus.Active && CurrentEnrollment > 0)
                throw new InvalidOperationException("Cannot update course information while students are enrolled");

            var previousTitle = _title;
            var previousDescription = _description;
            var previousMaxEnrollment = MaxEnrollment;

            _title = ValidateTitle(title);
            _description = ValidateDescription(description);
            MaxEnrollment = ValidateMaxEnrollment(maxEnrollment);
            
            MarkAsUpdated();

            // Raise domain event if anything changed
            if (previousTitle != _title || previousDescription != _description || previousMaxEnrollment != MaxEnrollment)
            {
                AddDomainEvent(new CourseUpdatedEvent(Id, _title, Code));
            }
        }

        /// <summary>
        /// Set prerequisite credit hours requirement
        /// </summary>
        /// <param name="creditHours">Required prerequisite credit hours</param>
        public void SetPrerequisiteRequirement(int creditHours)
        {
            if (creditHours < 0)
                throw new ArgumentException("Prerequisite credit hours cannot be negative", nameof(creditHours));

            PrerequisiteCreditHours = creditHours;
            MarkAsUpdated();
        }

        /// <summary>
        /// Enroll a student in the course
        /// </summary>
        public void EnrollStudent()
        {
            if (Status != CourseStatus.Active)
                throw new InvalidOperationException("Cannot enroll students in non-active course");

            if (CurrentEnrollment >= MaxEnrollment)
                throw new InvalidOperationException("Course is at maximum enrollment capacity");

            if (DateTime.Today > StartDate.AddDays(-7)) // Allow enrollment up to 1 week after start
                throw new InvalidOperationException("Enrollment period has ended");

            CurrentEnrollment++;
            MarkAsUpdated();

            // Raise domain event
            AddDomainEvent(new StudentEnrolledInCourseEvent(Id, Title, Code, CurrentEnrollment, MaxEnrollment));
        }

        /// <summary>
        /// Remove a student from the course
        /// </summary>
        public void WithdrawStudent()
        {
            if (CurrentEnrollment <= 0)
                throw new InvalidOperationException("No students enrolled to withdraw");

            CurrentEnrollment--;
            MarkAsUpdated();

            // Raise domain event
            AddDomainEvent(new StudentWithdrewFromCourseEvent(Id, Title, Code, CurrentEnrollment, MaxEnrollment));
        }

        /// <summary>
        /// Check if course has available spots
        /// </summary>
        public bool HasAvailableSpots() => CurrentEnrollment < MaxEnrollment;

        /// <summary>
        /// Get enrollment percentage
        /// </summary>
        public double GetEnrollmentPercentage() => MaxEnrollment > 0 ? (double)CurrentEnrollment / MaxEnrollment * 100 : 0;

        /// <summary>
        /// Check if course is full
        /// </summary>
        public bool IsFull() => CurrentEnrollment >= MaxEnrollment;

        /// <summary>
        /// Check if course is currently running
        /// </summary>
        public bool IsCurrentlyRunning() => Status == CourseStatus.Active && 
                                           DateTime.Today >= StartDate && 
                                           DateTime.Today <= EndDate;

        /// <summary>
        /// Check if enrollment is open
        /// </summary>
        public bool IsEnrollmentOpen() => Status == CourseStatus.Active && 
                                         DateTime.Today <= StartDate.AddDays(-7) && 
                                         HasAvailableSpots();

        /// <summary>
        /// Get course duration in days
        /// </summary>
        public int GetCourseDurationDays() => EndDate > StartDate ? (EndDate - StartDate).Days : 0;

        #region Validation Methods

        private static string ValidateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Course title cannot be empty", nameof(title));

            if (title.Length < 3)
                throw new ArgumentException("Course title must be at least 3 characters long", nameof(title));

            if (title.Length > 100)
                throw new ArgumentException("Course title cannot exceed 100 characters", nameof(title));

            return title.Trim();
        }

        private static string ValidateCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Course code cannot be empty", nameof(code));

            if (code.Length < 3)
                throw new ArgumentException("Course code must be at least 3 characters long", nameof(code));

            if (code.Length > 20)
                throw new ArgumentException("Course code cannot exceed 20 characters", nameof(code));

            // Course code should be alphanumeric
            if (!code.All(c => char.IsLetterOrDigit(c)))
                throw new ArgumentException("Course code can only contain letters and numbers", nameof(code));

            return code.Trim().ToUpperInvariant();
        }

        private static string ValidateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Course description cannot be empty", nameof(description));

            if (description.Length < 10)
                throw new ArgumentException("Course description must be at least 10 characters long", nameof(description));

            if (description.Length > 1000)
                throw new ArgumentException("Course description cannot exceed 1000 characters", nameof(description));

            return description.Trim();
        }

        private static int ValidateCreditHours(int creditHours)
        {
            if (creditHours <= 0)
                throw new ArgumentException("Credit hours must be greater than zero", nameof(creditHours));

            if (creditHours > 12)
                throw new ArgumentException("Credit hours cannot exceed 12", nameof(creditHours));

            return creditHours;
        }

        private static string ValidateDepartment(string department)
        {
            if (string.IsNullOrWhiteSpace(department))
                throw new ArgumentException("Department cannot be empty", nameof(department));

            if (department.Length < 2)
                throw new ArgumentException("Department must be at least 2 characters long", nameof(department));

            if (department.Length > 50)
                throw new ArgumentException("Department cannot exceed 50 characters", nameof(department));

            return department.Trim();
        }

        private static int ValidateMaxEnrollment(int maxEnrollment)
        {
            if (maxEnrollment <= 0)
                throw new ArgumentException("Maximum enrollment must be greater than zero", nameof(maxEnrollment));

            if (maxEnrollment > 500)
                throw new ArgumentException("Maximum enrollment cannot exceed 500", nameof(maxEnrollment));

            return maxEnrollment;
        }

        #endregion
    }

    /// <summary>
    /// Course level enumeration
    /// </summary>
    public enum CourseLevel
    {
        Undergraduate = 1,
        Graduate = 2,
        Postgraduate = 3,
        Doctoral = 4,
        Certificate = 5,
        Continuing = 6
    }

    /// <summary>
    /// Course status enumeration
    /// </summary>
    public enum CourseStatus
    {
        Draft = 1,
        Scheduled = 2,
        Active = 3,
        Inactive = 4,
        Completed = 5,
        Cancelled = 6,
        Archived = 7
    }
}
